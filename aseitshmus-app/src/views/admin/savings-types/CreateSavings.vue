<script setup>
    import { useStore } from 'vuex'
    import { useRouter } from 'vue-router';
    import { ref } from 'vue';
    import { useToast } from 'primevue/usetoast';
    import { required } from '@vuelidate/validators';
    import useVuelidate from '@vuelidate/core';
    import { dateOrderValidator } from '@/utils/DateUtils.js';
    import { errorMessage } from '@/constants/ErrorMessages.js';
    import { localeContent, buttonsContent  } from '@/constants/LabelContent.js';
    import { confirmationMessage } from '@/constants/ConfirmationMessages.js';

    const router = useRouter();
    const toast = useToast();
    const store = useStore();

    const loadingButton = ref(false);
    
    const typeList = () => { router.push({ name: "savingsList" }); }

    const selectedState = ref(1);
    const invalidStartDate = ref(false);
    const invalidEndDate = ref(false);

    const status = ref([{
            name: 'Activo',
            value: 1
        },
        {
            name: 'Inactivo',
            value: 2
        }
    ]);

    const savingsType = ref({
        Description: null,
        ApplicationDeadline: null,
        StartDate: null,
        EndDate: null,
        IsActive: selectedState
    })

    const rules = {
        Description: { required },
        ApplicationDeadline: { required },
        StartDate: { required },
        EndDate: { required },
        IsActive: { required },
    };

    const storeType = async () => {
        try {
            savingsType.value.StartDate = new Date(savingsType.value.StartDate);
            savingsType.value.EndDate = new Date(savingsType.value.EndDate);
            savingsType.value.ApplicationDeadline = new Date(savingsType.value.ApplicationDeadline);
            const type = {
            ...savingsType.value,
            IsActive: savingsType.value.IsActive === 2 ? 0 : savingsType.value.IsActive,
            };
            await store.dispatch('savingsTypes/addType', {
                savingsType: type
            })
        } catch (error) {
            toast.add({
                severity: 'error',
                detail: errorMessage.ERROR500,
                life: 3000
            });
        }
    }

    const v$ = useVuelidate(rules, savingsType);

    const validateForm = async () => {

        const applicationDeadline = new Date(savingsType.value.ApplicationDeadline);
    const startDate = new Date(savingsType.value.StartDate);
    const endDate = new Date(savingsType.value.EndDate);
        invalidStartDate.value = false;
        invalidEndDate.value = false;
        const result = await v$.value.$validate();
        if (!result) {
            if (v$.value.$errors[0].$validator === 'required') {
                toast.add({
                    severity: 'error',
                    detail: errorMessage.REQUIREDFIELDS,
                    life: 3000
                });
            }
            return false;
        } else if (!dateOrderValidator(applicationDeadline, startDate)) {
            invalidStartDate.value = true;
            toast.add({
                severity: 'error',
                detail: errorMessage.INVALIDSAVINGSSTARTDATE,
                life: 3000
            });
            return false
        } else if (!dateOrderValidator(startDate, endDate)) {
            invalidEndDate.value = true;
            toast.add({
                severity: 'error',
                detail: errorMessage.INVALIDSAVINGSENDDATE,
                life: 3000
            });
            return false
        }
        return true;
    };

    const submitData = async (event) => {
        event.preventDefault();
        const isValid = await validateForm();
        if (isValid) {
            try {
                try {
                        loadingButton.value = true;
                        await Promise.all([
                        storeType()              
                        ]);
                        toast.add({
                    severity: 'success',
                    detail: confirmationMessage.RECORDADDED,
                    life: 3000
                });
                await new Promise((resolve) => setTimeout(resolve, 1000));
                typeList();
                    } catch (error) {
                        console.error(error);
                    } finally {
                        loadingButton.value = false;
                    }       
            } catch (error) {
                console.error(error);
            }
        }
    }
</script>

<template>
    <div class="main">
        <toast-component />
        <div class="form">
            <div>
                <div class="form-row">
                    <div class="p-float-label">
                        <input-text placeholder="Tipo de ahorro" class=" input-text form-margin-right" id="typeName"
                            type="text" v-model="savingsType.Description"
                            :class="{'p-invalid': v$?.Description?.$error}" />
                        <label for="typeName">Tipo de ahorro</label>
                    </div>
                    <div class="p-float-label">
                        <date-picker v-model="savingsType.ApplicationDeadline" placeholder="Último día de inscripción"
                            class="dropdown " dateFormat="dd-mm-yy" showIcon id="last-day"
                            :class="{'p-invalid': v$?.ApplicationDeadline?.$error }" />
                        <label for="last-day">Último día de inscripción</label>
                    </div>

                    <div class="p-float-label form-margin-left">
                        <drop-down v-model="selectedState" :options="status" optionLabel="name" optionValue="value"
                        :emptyFilterMessage="localeContent.NOAVAILABLEOPTION" :emptyMessage="localeContent.NOAVAILABLEOPTION"
                            placeholder="Estado" class="dropdown" id="status"
                            :class="{'p-invalid': v$?.IsActive?.$error }" />
                        <label for="status">Estado</label>
                    </div>
                </div>
                <div class="form-row">
                    <div class="p-float-label">
                        <date-picker v-model="savingsType.StartDate" placeholder="Fecha de inicio"
                            class="dropdown form-margin-right" dateFormat="dd-mm-yy" showIcon id="start-day"
                            :class="{'p-invalid': v$?.StartDate?.$error || invalidStartDate} " />
                        <label for="start-day">Fecha de inicio</label>
                    </div>
                    <div class="p-float-label">
                        <date-picker v-model="savingsType.EndDate" placeholder="Fecha de finalización"
                            class="dropdown form-margin-right" dateFormat="dd-mm-yy" showIcon id="end-day"
                            :class="{'p-invalid': v$?.EndDate?.$error || invalidEndDate}" />
                        <label for="end-day">Fecha de finalización</label>
                    </div>
                </div>
            </div>
            <div class="actions">
                <base-button :label="buttonsContent.CANCEL" small @click="typeList" :type="'button'" />
                <base-button :label="buttonsContent.ADD" :loading="loadingButton" small @click="submitData" :type="'submit'" />
            </div>
        </div>
    </div>
</template>

<style scoped="scoped">
    .main {
        display: flex;
        justify-content: center;
        align-items: center;
        border: 1px solid #ebebeb;
        border-radius: 5px;
        padding: 2rem;
    }

    .form {
        display: flex;
        flex-direction: column;
        align-items: center;
        width: 100%;
    }

    .dropdownLarger {
        display: flex;
        width: 300px;
    }

    .form-row {
        margin-top: 6rem;
        display: flex;
        justify-content: space-between;
        align-self: center;
        margin-bottom: 2rem;
        width: 60%;
    }

    .form-margin-right {
        margin-right: 6rem;
    }

    .form-margin-left {
        margin-left: 6rem;
    }

    .actions {
        margin-top: 2rem;
        display: flex;
        flex-direction: row;
        justify-content: flex-end;
        align-self: flex-end;
    }

    .actions button {
        flex: 1;
        margin-right: 1rem;
    }

    .percentage-sign {
        position: absolute;
        top: 50%;
        right: 1rem;
        transform: translateY(-50%);
    }
</style>