<script setup>
    import { useStore } from 'vuex'
    import { useRouter, useRoute } from 'vue-router';
    import { ref, onMounted } from 'vue';
    import { useToast } from 'primevue/usetoast';
    import { required } from '@vuelidate/validators'
    import useVuelidate from '@vuelidate/core'
    import { errorMessage } from '@/constants/ErrorMessages.js'
    import { confirmationMessage } from '@/constants/ConfirmationMessages.js'
    import {  dateOrderValidator } from '@/utils/DateUtils.js'
    import Loading from 'vue-loading-overlay';
    import { localeContent, buttonsContent  } from '@/constants/LabelContent.js';
    import { useConfirm } from "primevue/useconfirm";

    const router = useRouter();
    const route = useRoute();
    const toast = useToast();
    const store = useStore();
    const confirm = useConfirm();

    const invalidStartDate = ref(false);
    const invalidEndDate = ref(false);
    const isLoading= ref(false);
    const loadingButton = ref(false);

    const typeList = () => { router.push({ name: "savingsList" }); }
 
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
        IsActive: null
    })

    const typeId = ref(route.params.id);
    
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

            await store.dispatch('savingsTypes/updateType', {
                typeId: typeId.value,
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
        const startDate = new Date(Date.parse(savingsType.value.StartDate));
        const applicationDeadline = new Date(Date.parse(savingsType.value.ApplicationDeadline));
        const endDate = new Date(Date.parse(savingsType.value.EndDate));
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
        } else if (!dateOrderValidator((startDate), (endDate))) {
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

    const fetchTypeData = async () => {
        await store.dispatch('savingsTypes/getTypeById', {
            rowId: typeId.value
        });
        const type = store.getters["savingsTypes/getType"];
        try {
                savingsType.value.Description = type.Description,
                savingsType.value.StartDate = new Date(Date.parse(type.StartDate));
                savingsType.value.ApplicationDeadline = new Date(Date.parse(type.ApplicationDeadline));
                savingsType.value.EndDate = new Date(Date.parse(type.EndDate));
                savingsType.value.IsActive = type.IsActive ? 1 : 2;
        } catch (error) {
            console.error(error);
            toast.add({
                severity: 'error',
                detail: errorMessage.ERROR500,
                life: 3000
            });
        }
    };

    const submitData = async () => {
            try {
                await storeType();
                toast.add({
                    severity: 'success',
                    detail: confirmationMessage.CHANGESSAVED,
                    life: 3000
                });
                await new Promise((resolve) => setTimeout(resolve, 1000));
                typeList()
            } catch (error) {
               console.error(error);
               toast.add({
                severity: 'error',
                detail: errorMessage.INVALIDSAVINGSSTARTDATE,
                life: 3000
            });
            }
        }

    const confirmUpdate = async (event) => {
        event.preventDefault();
        const isValid = await validateForm();
        if (isValid) {
            confirm.require({
                header: confirmationMessage.CONFIRMATIONHEADER,
                message: confirmationMessage.CONFIRMUPDATE,
                acceptLabel: localeContent.YES,
                icon: 'pi pi-exclamation-triangle',
                accept: async () => {
                    try {
                        loadingButton.value = true;
                        await Promise.all([
                            submitData()
                        ]);
                    } catch (error) {
                        console.error(error);
                    } finally {
                        loadingButton.value = false;
                    }
                },
                reject: () => {
                    toast.add({
                        severity: 'warn',
                        detail: confirmationMessage.TRANSACTIONCANCELLED,
                        life: 3000
                    });
                }
            });
        }
    };

    const loadingOverlay = async () => {
        isLoading.value = true;
        try {
            await Promise.all([
            fetchTypeData()
            ]);
        } catch (error) {
            console.error(error);
        } finally {
            isLoading.value = false;
        }
    }

    onMounted(async () => {
        await loadingOverlay();       
    })

</script>

<template>
    <loading v-model:active="isLoading" :lock-scroll="true" />
    <div class="main">
        <toast-component />
        <confirm-dialog></confirm-dialog> 
        <div class="form">
            <div>
                <div class="form-row">
                    <div class="p-float-label">
                        <input-text placeholder="Tipo de préstamo" class=" input-text form-margin-right" id="typeName"
                            type="text" v-model="savingsType.Description"
                            :class="{'p-invalid': v$?.LoanDescription?.$error}" />
                        <label for="typeName">Tipo de préstamo</label>
                    </div>
                    <div class="p-float-label">
                        <date-picker v-model="savingsType.ApplicationDeadline" placeholder="Último día de inscripción"
                            class="dropdown" dateFormat="dd-mm-yy" showIcon id="last-day"
                            :class="{'p-invalid': v$?.ApplicationDeadline?.$error }" />
                        <label for="last-day">Último día de inscripción</label>
                    </div>
                    <div class="p-float-label form-margin-left">
                        <drop-down v-model="savingsType.IsActive" :options="status" optionLabel="name"
                        :emptyFilterMessage="localeContent.NOAVAILABLEOPTION" :emptyMessage="localeContent.NOAVAILABLEOPTION"
                            optionValue="value" placeholder="Estado" class="dropdown" id="status"
                            :class="{'p-invalid': v$?.IsActive?.$error}" />
                        <label for="status">Estado</label>
                    </div>
                </div>
                <div class="form-row">
                    <div class="p-float-label">
                        <date-picker v-model="savingsType.StartDate" placeholder="Fecha de inicio"
                            class="dropdown form-margin-right" dateFormat="dd-mm-yy" showIcon id="start-day"
                            :class="{'p-invalid': v$?.StartDate?.$error || invalidStartDate}" />
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
                <base-button :label="buttonsContent.UPDATE" :loading="loadingButton" small @click="confirmUpdate" :type="'submit'" />
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
</style>