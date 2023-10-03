<script setup>
    import { ref, onMounted, computed, watch } from 'vue';
    import { useStore } from 'vuex';
    import { useRouter } from 'vue-router';
    import { useToast } from 'primevue/usetoast';
    import { FilterMatchMode, FilterOperator } from 'primevue/api';
    import { useConfirm } from "primevue/useconfirm";
    import { getDate } from '@/utils/DateUtils.js';
    import { getSubmissionStatus, getReviewStatus, getRequiresAction } from '@/utils/InputFieldsUtils.js'
    import { confirmationMessage } from '@/constants/ConfirmationMessages.js';
    import Loading from 'vue-loading-overlay';
    import { localeContent } from '@/constants/LabelContent.js';
    import { roles } from "@/constants/RolesConst.js";


    const router = useRouter();
    const store = useStore()
    const toast = useToast();
    const confirm = useConfirm();

    const isLoading= ref(false);
    const deletionStatus = ref(false);

    const role = computed(() => { return store.getters["auth/getRole"]; });

    const isAdmin = computed(() => (role.value === roles.ADMINISTRATOR));

    //const requiresEdit = computed(() => (role.value === roles.ADMINISTRATOR));

    const filters = ref({ global: { value: null, matchMode: FilterMatchMode.CONTAINS },        
        RequestedDate: { operator: FilterOperator.AND, constraints: [{ value: null, matchMode: FilterMatchMode.CONTAINS }] },
        LoanTypeName: { operator: FilterOperator.AND, constraints: [{ value: null, matchMode: FilterMatchMode.CONTAINS }] },     
        Name: { operator: FilterOperator.AND, constraints: [{ value: null, matchMode: FilterMatchMode.CONTAINS }] },
        LoanRequestId: { operator: FilterOperator.AND, constraints: [{ value: null, matchMode: FilterMatchMode.CONTAINS }] }
    
    });

    const requestData = computed(() => {
        const loans = store.getters["loanRequests/getLoans"];
        if (!Array.isArray(loans)) {
        return []; 
    }
        return loans.map(request => {
            return {
                ...request,       
            }
        });
    });

    const fetchRequestData = async () => {
        try {
            await store.dispatch('loanRequests/getAllLoans');
        } catch (error) {
            console.error(error.message);
        }
    }

    const storeRequest = async (id) => {
        await store.dispatch('loanRequests/deleteLoan', {
            rowId: id
        })
    }

    const deleteResponse = computed(() => {
        return store.getters["loanRequests/getErrorResponse"];
    });

    const deleteRecord = async (rowData) => {
        try {
            await storeRequest(rowData.data.LoanRequestId);
            if (deleteResponse.value === null) {
                toast.add({
                    severity: 'warn',
                    detail: confirmationMessage.RECORDDELETED,
                    life: 3000
                });
                deletionStatus.value = true;
            } else {
                toast.add({
                    severity: 'error',
                    detail: deleteResponse.value,
                    life: 3000
                });
                store.commit('loanRequests/clearErrorResponse');
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
            name: "updateLoanRequest",
            params: {
                id: rowData.data.LoanRequestId
            },
            props: true,
        });
    };

    const confirmDelete = async (rowData) => {
    confirm.require({
            message: confirmationMessage.CONFIRMDELETE,
            header: confirmationMessage.CONFIRMATIONHEADER,
            acceptLabel: localeContent.YES,
            icon: 'pi pi-exclamation-triangle',
            accept: async () => {
                try {
                    deleteRecord(rowData)                    
                } catch (error) {
                    console.error(error);
                } 
            },
            reject: () => {
                toast.add({ severity: 'warn', detail: confirmationMessage.TRANSACTIONCANCELLED, life: 3000 });
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
    <loading v-model:active="isLoading" :lock-scroll="true" />
    <toast-component />
    <confirm-dialog />
    <div class="list">
        <data-table :value="requestData" paginator :rows="6" tableStyle="max-width: 80rem" v-model:filters="filters"
            :globalFilterFields="['Name', 'LoanRequestId','LoanTypeName', 'RequestedDate']">
            <template #header>
                <div class="p-input-icon-right">
                    <input-text v-model="filters['global'].value"
                        placeholder="Código de solicitud, asociado, préstamo, fecha de solicitud" style="width: 200%;" />
                </div>
            </template>
            <data-column field="LoanRequestId" header="Código de solicitud" sortable />
            <data-column field="Name" header="Asociado" sortable  />
            <data-column field="LoanTypeName" header="Tipo de préstamo" sortable />
            <data-column field="RequestedDate" header="Fecha de solicitud" sortable>
                <template #body="rowData">
                    {{ getDate(rowData.data.RequestedDate )}}
                </template>
            </data-column>
            <data-column field="IsApproved" header="Estado de aprobación" sortable>
                <template #body="rowData">
                    {{ getSubmissionStatus(rowData.data.IsApproved) }}
                </template>
            </data-column>
            <data-column field="IsReviewRequired" header="Requiere revisión" sortable>
                <template #body="rowData">
                    {{ getRequiresAction(rowData.data.IsReviewRequired) }}
                </template>
            </data-column>
            <data-column field="IsReviewApproved" header="Estado de revisión" sortable>
                <template #body="rowData">
                    {{ getReviewStatus(rowData.data.IsReviewApproved, rowData.data.ReviewRequiredDate, rowData.data.IsReviewRequired) }}
                </template>
            </data-column>
            <data-column header="" style="width: 150px"> <template #body="rowData">
                    <base-button class="action-buttons" label="Más detalle" @click="updateRecord(rowData)"
                        :type="'button'" />
                </template></data-column>
            <data-column header="" style="width: 150px" v-if="isAdmin"> 
                <template #body="rowData">
                    <base-button class="action-buttons" label="Eliminar" color="red" @click="confirmDelete(rowData)"
                        :type="'button'" />
                </template>
            </data-column>
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