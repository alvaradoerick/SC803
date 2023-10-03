<script setup>
    import { ref, onMounted, computed, watch } from 'vue';
    import { useStore } from 'vuex';
    import { useRouter } from 'vue-router';
    import { useToast } from 'primevue/usetoast';
    import { FilterMatchMode, FilterOperator } from 'primevue/api';
    import { useConfirm } from "primevue/useconfirm";
    import Loading from 'vue-loading-overlay';
    import { errorMessage } from "@/constants/ErrorMessages.js";
    import { confirmationMessage } from "@/constants/ConfirmationMessages.js";
    import { statusName } from '@/utils/InputFieldsUtils.js';
    import { localeContent, buttonsContent } from '@/constants/LabelContent.js';


    const router = useRouter();
    const store = useStore()
    const toast = useToast();
    const confirm = useConfirm();
    const isLoading= ref(false);
    const categoryData = ref([]);
    const deletionStatus = ref(false);

    const filters = ref(
    {
        global: { value: null, matchMode: FilterMatchMode.CONTAINS },
        Description: { operator: FilterOperator.AND, constraints: [{ value: null, matchMode: FilterMatchMode.CONTAINS }]},
    }
    );

    const fetchCategoryData = async () => {
        await store.dispatch('categories/getAllCategories');
        categoryData.value = store.getters['categories/getCategory'];
    };

    const storeCategory = async (id) => {
        await store.dispatch('categories/deleteCategory', {
            rowId: id
        })
    }

    const deleteResponse = computed(() => {
        return store.getters["categories/getErrorResponse"];
    });

    const deleteRecord = async (rowData) => {
        try {
            await storeCategory(rowData.data.CategoryAgreementId);
            if (deleteResponse.value === null) {
                toast.add({
                    severity: 'warn',
                    detail:  confirmationMessage.RECORDDELETED,
                    life: 3000
                });
                deletionStatus.value = true;
            } else {
                toast.add({
                    severity: 'error',
                    detail: deleteResponse.value,
                    life: 3000
                });
                store.commit('categories/clearErrorResponse');
            }
        } catch (error) {
            toast.add({
                severity: 'error',
                detail: errorMessage.ERROR500,
                life: 3000
            });
        }
    };

    const confirmDelete = (rowData) => {
        confirm.require({
            message: confirmationMessage.CONFIRMDELETE,
            header: confirmationMessage.CONFIRMATIONHEADER,
            acceptLabel: localeContent.YES,
            icon: 'pi pi-exclamation-triangle',
            accept: () => {
                deleteRecord(rowData);
            },
            reject: () => {
                toast.add({
                    severity: 'warn',
                    detail: 'Se canceló la transacción.',
                    life: 3000
                });
            }
        });
    };

    watch(deletionStatus, (newStatus) => {
        if (newStatus) {
            fetchCategoryData();
            deletionStatus.value = false;
        }
    });

    const addCategory = () => {
        router.push({ name: "createCategory" });
    }

    const updateCategory = (rowData) => {
        router.push({
            name: "updateCategory",
            params: {
                id: rowData.data.CategoryAgreementId
            },
            props: true,
        });
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
    <toast-component />
    <loading v-model:active="isLoading" :lock-scroll="true" />
    <confirm-dialog></confirm-dialog>   
    <div class="category-list">
        <div class="actions-container">
            <div class="actions">
                <base-button :label="buttonsContent.ADD" small @click="addCategory" :type="'button'" color="green" />
            </div>
        </div>
        <data-table v-model:filters="filters" :value="categoryData" paginator :rows="6" tableStyle="max-width: 70rem"  
        :globalFilterFields="['Description']">
            <template #header>
                    <div class="p-input-icon-right">
                    <input-text v-model="filters['global'].value" placeholder="Categoría" style="width: 80%;"/>
                    </div>
            </template>
            <data-column field="Description" header="Categoría" sortable></data-column>
            <data-column field="IsActive" header="Estado" sortable style="width: 160px">
                <template #body="slotProps">
                    {{ statusName(slotProps.data.IsActive) }}
                </template>
            </data-column>
            <data-column header="" style="width: 100px"> <template #body="rowData">
                    <base-button class="action-buttons" label="Editar" @click="updateCategory(rowData)"
                        :type="'button'" />
                </template></data-column>

            <data-column header="" style="width: 100px"> <template #body="rowData">
                    <base-button class="action-buttons" color="red" label="Eliminar" @click="confirmDelete(rowData) "
                        :type="'button'" />
                </template></data-column>
        </data-table>


    </div>
</template>

<style scoped="scoped">
    .category-list {
        display: flex;
        flex-direction: column;
        align-items: center;
        width: 100%;
    }

    .action-buttons {
        display: flex;
        overflow: hidden;
        width: 105px;
        color: white;
        text-align: center;
        flex-direction: column;
        align-items: center;
    }

    .actions-container {
        position: static;
        bottom: 0;
        background-color: #fff;
        width: 100%;
        justify-content: center;
    }

    .actions {
        display: flex;
        flex: 1;
        align-items: center;
        margin-top: 2rem;
        justify-content: flex-end;
        margin-bottom: 1rem
    }
</style>