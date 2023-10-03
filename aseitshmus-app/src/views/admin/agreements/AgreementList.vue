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
    import { localeContent, buttonsContent } from '@/constants/LabelContent.js';

    const router = useRouter();
    const store = useStore()
    const toast = useToast();
    const confirm = useConfirm();
    const isLoading= ref(false);
    const agreementData = ref([]);
    const deletionStatus = ref(false);

    const filters = ref({
        global: { value: null, matchMode: FilterMatchMode.CONTAINS },
        Title: { operator: FilterOperator.AND, constraints: [{ value: null, matchMode: FilterMatchMode.CONTAINS }] },
        CategoryName: { operator: FilterOperator.AND, constraints: [{ value: null, matchMode: FilterMatchMode.CONTAINS }] }
    });

    const fetchAgreementData = async () => {
        await store.dispatch('agreements/getAllAgreements');
        const agreements = store.getters['agreements/getAgreement'];
        agreementData.value = agreements.map(agreement => {
            return {
                ...agreement,
            };
        });
    };

    const storeAgreement = async (id) => {
        await store.dispatch('agreements/deleteAgreement', {
            rowId: id
        })
    }

    const deleteResponse = computed(() => {
        return store.getters["agreements/getErrorResponse"];
    });

    const deleteRecord = async (rowData) => {
        try {
            await storeAgreement(rowData.data.AgreementId);
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
                store.commit('agreements/clearErrorResponse');
            }
        } catch (error) {
            toast.add({
                severity: 'error',
                detail: errorMessage.ERROR500,
                life: 3000
            });
        }
    };

    watch(deletionStatus, (newStatus) => {
        if (newStatus) {
            fetchAgreementData();
            deletionStatus.value = false;
        }
    });

    const addAgreement = () => {
        router.push({
            name: "createAgreement"
        });
    }
    const updateCategory = (rowData) => {
        router.push({
            name: "updateAgreement",
            params: {
                id: rowData.data.AgreementId
            },
            props: true,
        });
    };

    const getStatus = (value) => {   
        return value ? 'Activo' : 'Inactivo';
    }

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
            fetchAgreementData()
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
    <div class="agreement-list">
        <div class="actions-container">
            <div class="actions">
                <base-button :label="buttonsContent.ADD" small @click="addAgreement" :type="'button'" color="green" />
            </div>
        </div>
        <data-table v-model:filters="filters" :value="agreementData" tableStyle="max-width: 80rem" paginator :rows="6"
            :globalFilterFields="['Title','CategoryName']">
            <template #header>
                <div class="p-input-icon-right">
                    <input-text v-model="filters['global'].value" placeholder="Convenio, categoría" style="width: 90%; " />
                </div>
            </template>
            <data-column field="Title" header="Convenio" sortable></data-column>
            <data-column field="CategoryName" header="Categoría" sortable style="width: 300px"/>
            <data-column field="IsActive" header="Estado" sortable style="width: 180px">
                <template #body="slotProps">
                    {{ getStatus(slotProps.data.IsActive) }}
                </template></data-column>
            <data-column header="" style="width: 100px">
                <template #body="rowData">
                    <base-button class="action-buttons" label="Editar" :type="'button'"
                        @click="updateCategory(rowData)" />
                </template>
            </data-column>
            <data-column header="" style="width: 100px"> <template #body="rowData">
                    <base-button class="action-buttons" label="Eliminar" color="red" @click="confirmDelete(rowData)"
                        :type="'button'" />
                </template></data-column>
        </data-table>
    </div>
</template>


<style scoped="scoped">
    .agreement-list {
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