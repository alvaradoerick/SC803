<script setup>
    import useVuelidate from '@vuelidate/core'
    import { required } from '@vuelidate/validators'
    import { useStore } from 'vuex'
    import { useRouter } from 'vue-router';
    import { ref, } from 'vue';
    import { useToast } from 'primevue/usetoast';
    import { errorMessage } from '@/constants/ErrorMessages';
    import { confirmationMessage } from '@/constants/ConfirmationMessages.js';
    import { localeContent, buttonsContent } from '@/constants/LabelContent.js';



    const store = useStore();
    const router = useRouter();
    const toast = useToast();
    const categoryList = () => {
        router.push({
            name: "categoryList"
        });
    }
    const loadingButton = ref(false);

    const selectedState = ref(1);
    const status = ref([{
            name: 'Activo',
            value: 1
        },
        {
            name: 'Inactivo',
            value: 2
        }
    ]);

    const agreementCategory = ref({
        Description: null,
        IsActive: selectedState
    })

    const rules = {
        Description: { required },
        IsActive: { required }
    }

    const v$ = useVuelidate(rules, agreementCategory);

    const validateForm = async () => {
        const result = await v$.value.$validate();
        if (!result) {
            if (v$.value.$errors[0].$validator === 'required') {
                toast.add({
                    severity: 'error',
                    detail: errorMessage.REQUIREDFIELDS,
                    life: 3000
                });
            }
            return false
        }
        return true;
    }

    const storeCategory = async () => {
        const category = {
      ...agreementCategory.value,
      IsActive: agreementCategory.value.IsActive === 2 ? 0 : agreementCategory.value.IsActive,
    };
        await store.dispatch('categories/addCategory', {
            agreementCategory: category,
        })
    }

    const onSend = async (event) => {
        event.preventDefault();
        const isValid = await validateForm();
        if (isValid) {
            try {
                try {
                        loadingButton.value = true;
                        await Promise.all([
                        storeCategory()              
                        ]);
                        toast.add({
                    severity: 'success',
                    detail: confirmationMessage.RECORDADDED,
                    life: 3000
                });
                await new Promise((resolve) => setTimeout(resolve, 1000));
                router.push({ name: 'categoryList' });
                    } catch (error) {
                        console.error(error);
                    } finally {
                        loadingButton.value = false;
                    }             
            } catch (error) {
                console.error(error)
                toast.add({
                    severity: 'error',
                    detail: errorMessage.ERROR500,
                    life: 3000
                });
            }
        }
    }
</script>

<template>
    <div class="main">
        <toast-component />
        <div class="form">
            <div class="header">
                <div class="form-row">
                    <div class="p-float-label">
                        <input-text placeholder="Nombre" class=" input-text form-margin-right" id="categoryName"
                            type="text" v-model="agreementCategory.Description"
                            :class="{'p-invalid': v$?.Description?.$error}" />
                        <label for="categoryName">Nombre</label>
                    </div>
                    <div class="p-float-label">
                        <drop-down v-model="selectedState" :options="status" optionLabel="name" optionValue="value" :emptyFilterMessage="localeContent.NOAVAILABLEOPTION" :emptyMessage="localeContent.NOAVAILABLEOPTION"
                            id="state" placeholder="Estado" class="dropdown"
                            :class="{'p-invalid': v$?.selectedState?.$error}" />
                        <label for="state">Estado</label>
                    </div>
                </div>
            </div>
            <div class="actions">
                <base-button :label="buttonsContent.CANCEL" small @click="categoryList" :type="'button'" />
                <base-button :label="buttonsContent.ADD" :loading="loadingButton" small @click="onSend" :type="'submit'" />
            </div>
        </div>
    </div>
</template>

<style scoped>
    .main {
        display: flex;
        justify-content: center;
        align-items: center;
        border: 1px solid #ebebeb;
        border-radius: 5px;
        margin: 1rem;
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
        margin-top: 2rem;
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