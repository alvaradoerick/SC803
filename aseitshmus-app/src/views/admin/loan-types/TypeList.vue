<script setup>
    import { ref, onMounted, computed, watch } from 'vue';
    import { useStore } from 'vuex';
    import { useRouter } from 'vue-router';
    import { useToast } from 'primevue/usetoast';
    import { FilterMatchMode, FilterOperator } from 'primevue/api';
    import { useConfirm } from "primevue/useconfirm";
    import { confirmationMessage } from '@/constants/ConfirmationMessages.js';
    import Loading from 'vue-loading-overlay';
    import { localeContent, buttonsContent } from '@/constants/LabelContent.js';


    const router = useRouter();
    const store = useStore()
    const toast = useToast();
    const confirm = useConfirm();

    const filters = ref({
        global: { value: null, matchMode: FilterMatchMode.CONTAINS },
        Description: { operator: FilterOperator.AND, constraints: [{ value: null, matchMode: FilterMatchMode.CONTAINS }] }, 
        Term: { operator: FilterOperator.AND, constraints: [{ value: null, matchMode: FilterMatchMode.CONTAINS }] }
    });
    const isLoading= ref(false);
    const typeData = ref([]);
    const deletionStatus = ref(false);

    const fetchTypeData = async () => {
        await store.dispatch('loanTypes/getAllTypes');
        const types = store.getters['loanTypes/getType'];
        typeData.value = types.map(type => {
            return {
                ...type,
                IsActive: type.IsActive ? "Activo" : "Inactivo"
            };
        });
    };

    const storeType = async (id) => {
        await store.dispatch('loanTypes/deleteType', {
            rowId: id
        })
    }

    const deleteResponse = computed(() => {
        return store.getters["loanTypes/getErrorResponse"];
    });

    const deleteRecord = async (rowData) => {
        try {
            await storeType(rowData.data.LoansTypeId);
            if (deleteResponse.value === null) {
                toast.add({
                    severity: 'warn',
                    detail: "Tipo de préstamo ha sido eliminado.",
                    life: 3000
                });
                deletionStatus.value = true;
            } else {
                toast.add({
                    severity: 'error',
                    detail: deleteResponse.value,
                    life: 3000
                });
                store.commit('loanTypes/clearErrorResponse');
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
            fetchTypeData();
            deletionStatus.value = false;
        }
    });

    const addRecord = () => {
        router.push({
            name: "createType"
        });
    }

    const updateRecord = (rowData) => {
        router.push({
            name: "updateType",
            params: {
                id: rowData.data.LoansTypeId
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
    <toast-component />
    <loading v-model:active="isLoading" :lock-scroll="true" />
    <confirm-dialog/>
    <div class="list">
        <div class="actions-container">
            <div class="actions">
                <base-button :label="buttonsContent.ADD" @click="addRecord" small :type="'button'" color="green" />
            </div>
        </div>
        <data-table v-model:filters="filters" :value="typeData" paginator :rows="6" tableStyle="max-width: 100rem"
            :globalFilterFields="['Description','Term']">
            <template #header>
                <div class="p-input-icon-right">
                    <input-text v-model="filters['global'].value" placeholder="Tipo de préstamo, plazo" style="width: 100%;" />
                </div>
            </template>
            <data-column field="Description" header="Tipo de préstamo" sortable style="width: 50%"></data-column>
            <data-column field="Term" header="Plazo (meses)" sortable ></data-column>
            <data-column field="IsActive" header="Estado" sortable style="width: 20%" ></data-column>
            <data-column header="" style="width: 100px"> <template #body="rowData">
                    <base-button class="action-buttons" label="Editar" @click="updateRecord(rowData)"
                        :type="'button'" />
                </template></data-column>
            <data-column header="" style="width: 100px"> <template #body="rowData">
                    <base-button class="action-buttons" label="Eliminar" color="red" @click="confirmDelete(rowData)"
                        :type="'button'" />
                </template></data-column>
        </data-table>
    </div>
</template>

<style scoped="scoped">
    .clist {
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
        justify-content: flex-end;
        margin-bottom: 1rem
    }
</style>