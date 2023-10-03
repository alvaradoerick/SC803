<script setup>
    import { ref, onMounted, computed } from 'vue';
    import { useStore } from 'vuex';
    import { useRouter } from 'vue-router';
    import { FilterMatchMode, FilterOperator } from 'primevue/api';
    import Loading from 'vue-loading-overlay';
    import { getDate } from '@/utils/DateUtils.js';
    import { buttonsContent } from '@/constants/LabelContent.js';

    const router = useRouter();
    const store = useStore()

    const usersData = ref([]);
    const isLoading= ref(false);
    const filters = ref(
    {
        global: { value: null, matchMode: FilterMatchMode.CONTAINS },
        PersonId: { operator: FilterOperator.AND, constraints: [{ value: null, matchMode: FilterMatchMode.CONTAINS }]},
        FirstName: { operator: FilterOperator.AND, constraints: [{ value: null, matchMode: FilterMatchMode.CONTAINS }]},
        LastName1: { operator: FilterOperator.AND, constraints: [{ value: null, matchMode: FilterMatchMode.CONTAINS }]},
        LastName2: { operator: FilterOperator.AND, constraints: [{ value: null, matchMode: FilterMatchMode.CONTAINS }]},
        NumberId: { operator: FilterOperator.AND, constraints: [{ value: null, matchMode: FilterMatchMode.CONTAINS }] },
        WorkStartDate: { operator: FilterOperator.AND, constraints: [{ value: null, matchMode: FilterMatchMode.CONTAINS }]},
        IsActive: { operator: FilterOperator.AND, constraints: [{ value: null, matchMode: FilterMatchMode.CONTAINS }]},
    }
    );

    const getStatus = (value) => {   
        return value ? 'Activo' : 'Inactivo';
    }

    const getFullName =  (firstName , lastName1, lastName2) => {
          if   (lastName2 == null) {
            return `${firstName} ${lastName1}`
          }
          else {
            return `${firstName} ${lastName1} ${lastName2}`
          }
    } 

    const fetchUsersData = async () => {
        await store.dispatch('users/getAll');
        usersData.value = store.getters['users/getUsers'];      
    };
 
    const sortedUsersData = computed(() => {
        return [...usersData.value].sort((a, b) => {
            if (a.IsActive === b.IsActive) {
                return new Date(a.WorkStartDate) - new Date(b.WorkStartDate);
            }
            return a.IsActive === 'Inactivo' ? -1 : 1;
        });
    });

    const updateUser = (rowData) => {
        router.push({
            name: "updateUser",
            params: {
                id: rowData.data.PersonId
            },
            props: true,
        });
    };

    const loadingOverlay = async () => {
        isLoading.value = true;
        try {
            await Promise.all([
            fetchUsersData()
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
    <div class="user-list">
        <data-table v-model:filters="filters" :value="sortedUsersData" paginator :rows="6" tableStyle="max-width: 90rem"
            :globalFilterFields="['PersonId','FullName', 'NumberId', 'WorkStartDate', 'IsActive', 'FirstName', 'LastName1', 'LastName2']">
            <template #header>
                    <div class="p-input-icon-right">
                    <input-text v-model="filters['global'].value" placeholder="Código, asociado, identificación" style="width: 130%;"/>
                    </div>
            </template>
            <data-column field="PersonId" header="Código de empleado" sortable style="width: 250px"></data-column>
            <data-column field="FullName" header="Asociado" sortable>
                <template #body="slotProps">
                    {{ getFullName(slotProps.data.FirstName,slotProps.data.LastName1,slotProps.data.LastName2) }}
                </template>
            </data-column>
            <data-column field="NumberId" header="Identificación" sortable style="width: 200px"></data-column>
            <data-column field="WorkStartDate" header="Fecha de ingreso" sortable style="width: 200px">
                <template #body="slotProps">
                    {{ getDate(slotProps.data.WorkStartDate) }}
                </template></data-column>
            <data-column field="IsActive" header="Estado" sortable >
                <template #body="slotProps">
                    {{ getStatus(slotProps.data.IsActive) }}
                </template>
            </data-column>
            <data-column header="" style="width: 150px"> 
                <template #body="rowData">
                    <base-button class="action-buttons" :label="buttonsContent.MOREDETAILS" @click="updateUser(rowData)" :type="'button'" />
                </template></data-column>
        </data-table>

    </div>
</template>

<style scoped="scoped">
    .user-list {
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
        justify-content: center;
    }

    .actions-container {
        position: static;
        bottom: 0;
        background-color: #fff;
        width: 100%;
    }
    .actions {
        display: flex;
        flex: 1;
        align-items: center;
        margin-top: 2rem;
    }

    .search-box{
        position:absolute; 
        right:0
    }
</style>