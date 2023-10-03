<script setup>
    import { useStore } from 'vuex'
    import { useRouter, useRoute } from 'vue-router';    
    import { ref, onMounted } from 'vue';
    import { useToast } from 'primevue/usetoast';
    import { required } from '@vuelidate/validators'
    import useVuelidate from '@vuelidate/core'
    import Loading from 'vue-loading-overlay';
    import { confirmationMessage } from '@/constants/ConfirmationMessages.js';
    import { errorMessage } from '@/constants/ErrorMessages.js';
    import { useConfirm } from "primevue/useconfirm";
    import { localeContent, buttonsContent } from '@/constants/LabelContent.js';

    const store = useStore();
    const router = useRouter();
    const route = useRoute();
    const toast = useToast();
    const confirm = useConfirm();

    const loadingButton = ref(false);
    const isLoading= ref(false);
    const categoryId = ref(route.params.id);

    const categoryList = () => {
        router.push({ name: "categoryList" });
    }
      
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
        IsActive: 2
    })

    const rules = {
        Description: { required },
        IsActive: { required }
    }
    
    const storeCategory = async () => {
        const category = {
      ...agreementCategory.value,
      IsActive: agreementCategory.value.IsActive === 2 ? 0 : agreementCategory.value.IsActive,
    };

        await store.dispatch('categories/updateCategory', {
            categoryId: categoryId.value,
            agreementCategory: category
        })
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


    const fetchCategoryData = async () => {
        try {
            await store.dispatch('categories/getCategoryById', {
                rowId: categoryId.value
            });
            const category = store.getters["categories/getCategory"];
            agreementCategory.value.Description = category.Description,
                agreementCategory.value.IsActive = category.IsActive ? 1 : 2;
        } catch (error) {
            console.error(error)
            toast.add({
                severity: 'error',
                detail: errorMessage.ERROR500,
                life: 3000
            });
        }
    };

    const submitData = async () => {
        try {
            await storeCategory();
            toast.add({
                severity: 'success',
                detail: confirmationMessage.CHANGESSAVED,
                life: 3000
            });
            await new Promise((resolve) => setTimeout(resolve, 1000));
            router.push({ name: 'categoryList' });
        } catch (error) {
            console.error(error)
            toast.add({
                severity: 'error',
                detail: errorMessage.ERROR500,
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
                fetchCategoryData()
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
    <div class="main">
        <confirm-dialog></confirm-dialog>  
        <loading v-model:active="isLoading" :lock-scroll="true" />
        <toast-component />
        <div class="form">
            <div>
                <div class="form-row">
                    <div class="p-float-label">
                        <input-text placeholder="Nombre" class=" input-text form-margin-right" id="categoryName"
                            type="text" v-model="agreementCategory.Description"
                            :class="{'p-invalid': v$?.Description?.$error}" />
                        <label for="categoryName">Nombre</label>
                    </div>
                    <div class="p-float-label">
                        <drop-down v-model="agreementCategory.IsActive" :options="status" optionLabel="name" :emptyFilterMessage="localeContent.NOAVAILABLEOPTION" :emptyMessage="localeContent.NOAVAILABLEOPTION"
                            optionValue="value" placeholder="Estado" class="dropdown" id="status"
                            :class="{'p-invalid': v$?.IsActive?.$error}" />
                        <label for="status" :class="{'float-label': agreementCategory.IsActive !== 0}">Estado</label>

                    </div>
                </div>
            </div>
            <div class="actions">
                <base-button :label="buttonsContent.CANCEL" :small="true" @click="categoryList" :type="'button'" />
                <base-button :label="buttonsContent.UPDATE" :loading="loadingButton"  :small="true" @click="confirmUpdate" :type="'submit'" />
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