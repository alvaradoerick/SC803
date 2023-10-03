<script setup>
    import { ref, onMounted, computed, watch } from 'vue';
    import { useStore } from 'vuex';
    import { useRouter } from 'vue-router';
    import { useToast } from 'primevue/usetoast';
    import { FilterMatchMode, FilterOperator } from 'primevue/api';
    import { useConfirm } from "primevue/useconfirm";
    import { confirmationMessage } from '@/constants/ConfirmationMessages.js';
    import Loading from 'vue-loading-overlay';
    import { getDate } from '@/utils/DateUtils.js';
    import { localeContent } from '@/constants/LabelContent.js';

    const router = useRouter();
    const store = useStore()
    const toast = useToast();
    const confirm = useConfirm();

    const deletionStatus = ref(false);
    const isLoading= ref(false);
     
    const filters = ref({ global: { value: null, matchMode: FilterMatchMode.CONTAINS },
        Name: { operator: FilterOperator.AND, constraints: [{ value: null, matchMode: FilterMatchMode.CONTAINS }] },
        SavingsTypeName: { operator: FilterOperator.AND, constraints: [{ value: null, matchMode: FilterMatchMode.CONTAINS }] },
        ApplicationDate: { operator: FilterOperator.AND, constraints: [{ value: null, matchMode: FilterMatchMode.CONTAINS }] },
        SavingsRequestId: { operator: FilterOperator.AND, constraints: [{ value: null, matchMode: FilterMatchMode.CONTAINS }] }
    });

    const requestData = computed(() => {
        const savings = store.getters["savingsRequests/getSavings"];
        if (!Array.isArray(savings)) {
        return []; 
    }
        return savings.map(request => {
            return {
                ...request,
                IsActive: request.IsActive ? "Activo" : "Inactivo",  
                status: getStatusForRequest(request),     
            }
        });
    });

    const getStatusForRequest = (request) => {
        if (request.IsCanceled) {
            return 'Cancelado';
        } else if (request.IsActive) {
            return 'Aprobado';
        } else if (request.IsApproved === null) {
            return 'Pendiente';
        } else {
            return 'Rechazado';
        }
    };

    const fetchRequestData = async () => {
        try {
            await store.dispatch('savingsRequests/getAllSavings');
        } catch (error) {
            toast.add({
                severity: 'error',
                detail: error,
                life: 3000
            });
        }
    }

    const storeRequest = async (id) => {
        await store.dispatch('savingsRequests/deleteSavings', {
            rowId: id
        })
    }

    const deleteResponse = computed(() => {
        return store.getters["savingsRequests/getErrorResponse"];
    });

    const deleteRecord = async (rowData) => {
        try {
            await storeRequest(rowData.data.SavingsRequestId);
            if (deleteResponse.value === null) {
                toast.add({
                    severity: 'warn',
                    detail: "Solicitud de ahorro ha sido eliminada.",
                    life: 3000
                });
                deletionStatus.value = true;
            } else {
                toast.add({
                    severity: 'error',
                    detail: deleteResponse.value,
                    life: 3000
                });
                store.commit('savingsRequests/clearErrorResponse');
            }
        } catch (error) {
            toast.add({
                severity: 'error',
                detail: error,
                life: 3000
            });
        }
    };

  
    watch(deletionStatus, (newStatus) => {
        if (newStatus) {
            fetchRequestData();
            deletionStatus.value = false;
        }
    });

    const updateRecord = (rowData) => {
        router.push({
            name: "updateSavingRequest",
            params: {
                id: rowData.data.SavingsRequestId
            },
            props: true,
        });
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
            toast.add({ severity: 'warn', detail: 'Se canceló la transacción.', life: 3000 });
        }
    });
    };

    const loadingOverlay = async () => {
        isLoading.value = true;
        try {
            await Promise.all([
            fetchRequestData()
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
    <confirm-dialog/>
    <div class="list">
        <data-table :value="requestData" paginator :rows="6" tableStyle="min-width: 80rem" v-model:filters="filters"
            :globalFilterFields="['SavingsRequestId', 'Name','SavingsTypeName', 'ApplicationDate']">
            <template #header>
                <div class="p-input-icon-right">
                    <input-text v-model="filters['global'].value"
                        placeholder="Código de solicitud, asociado, tipo de ahorro, fecha de solicitud" style="width: 220%;" />
                </div>
            </template>
            <data-column field="SavingsRequestId" header="Código de solicitud" sortable />
            <data-column field="Name" header="Asociado" sortable></data-column>
            <data-column field="SavingsTypeName" header="Tipo de ahorro" sortable></data-column>
            <data-column field="ApplicationDate" header="Fecha de solicitud" sortable>
                <template #body="rowData">
                    {{ getDate(rowData.data.ApplicationDate )}}
                </template>
            </data-column>
            <data-column field="IsActive" header="Estado" />
           
            <data-column field="status" header="Estado de aprobación" sortable>
                <template #body="rowData">
                    {{ rowData.data.status }}
                </template>
            </data-column>
            <data-column header="" style="width: 150px"> <template #body="rowData">
                    <base-button class="action-buttons" label="Más detalle"  @click="updateRecord(rowData)"
                        :type="'button'" />
                </template></data-column>
            <data-column header="" style="width: 150px"> <template #body="rowData">
                    <base-button class="action-buttons" label="Eliminar"  color="red" @click="confirmDelete(rowData)"
                        :type="'button'" />
                </template></data-column>
        </data-table>
    </div>
</template>

<style scoped="scoped">
    .list {
        display: flex;
        flex-direction: column;
        align-items: center;
        width: 100%;
    }

    .action-buttons {
        display: flex;
        overflow: hidden;
        width: 115px;
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
        justify-content: space-between;
    }
</style>