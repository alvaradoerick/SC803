<script setup>
    import { ref, onMounted } from 'vue';
    import { useRouter } from 'vue-router';
    import { useStore } from 'vuex'
    import { getDate } from '@/utils/DateUtils.js';
    import Loading from 'vue-loading-overlay';
    import { buttonsContent } from '@/constants/LabelContent.js';

    const router = useRouter();
    const store = useStore();

    const isLoading= ref(false);
    const pendingUsers =  ref([]);
    const pendingLoans =  ref([]);
    const pendingSavings =  ref([]);
    const pendingUsersCount =  ref(null);
    const pendingUsersLastRequestDate =  ref(null);
    const pendingLoansCount =  ref(null);
    const pendingLoansLastRequestDate =  ref(null);
    const pendingSavingsCount =  ref(null);
    const pendingSavingsLastRequestDate =  ref(null);
    let activeUsersCount;
    let activeLoansCount;
    let activeSavingsCount;

    const fetchSavings = async () => {
        try {
            const response = await store.dispatch('savingsRequests/getAllSavings');
            if (response) {
                const savingsData = Array.isArray(response) ? response : [response].filter(Boolean);
                activeSavingsCount = savingsData.filter(savings => savings.IsActive == true).length;
                pendingSavings.value = savingsData.filter(savings => savings.IsActive == false && savings.ApprovedDate == null);
                pendingSavingsCount.value = pendingSavings.value.length;
                pendingSavings.value = pendingSavings.value.map(savings => ({
            ...savings,
            ApplicationDate: getDate(savings.ApplicationDate),
          }));
                if (pendingSavings.value.length > 0) {
                    pendingSavingsLastRequestDate.value = pendingSavings.value[0].ApplicationDate;
                } else {
                    pendingSavingsLastRequestDate.value = null;
                }
            } else {
                console.error("No se encontró ningun registro.");
            }
        } catch (error) {
            console.error(error.message);
        }
    };
 
    const fetchUsers = async () => {
        try {
            const response = await store.dispatch('users/getAll');
            if (response) {
                const usersData = Array.isArray(response.data) ? response.data : [response.data].filter(Boolean);
                activeUsersCount = usersData.filter(user => user.IsActive == true).length;
                pendingUsers.value = usersData.filter(user => user.IsActive == false && user.ApprovedDate == null);
                pendingUsersCount.value = pendingUsers.value.length;
                pendingUsers.value = pendingUsers.value.map(user => ({
            ...user,
            RequestedDate: getDate(user.RequestedDate),
          }));
                if (pendingUsers.value.length > 0) {
                    pendingUsersLastRequestDate.value = pendingUsers.value[0].RequestedDate;
                } else {
                    pendingUsersLastRequestDate.value = null;
                }
            } else {
                console.error("No se encontró ningun registro.");
            }
        } catch (error) {
            console.error(error.message);
        }
    };
    
    const fetchLoans = async () => {
        try {
            const response = await store.dispatch('loanRequests/getAllLoans');
            if (response) {
                const loansData = Array.isArray(response) ? response : [response].filter(Boolean);
                activeLoansCount = loansData.filter(user => user.IsActive == true).length;
                pendingLoans.value = loansData.filter(user => user.IsActive == false && user.ApprovedDate == null);
                pendingLoansCount.value = pendingLoans.value.length;
                pendingLoans.value = pendingLoans.value.map(loan => ({
            ...loan,
            RequestedDate: getDate(loan.RequestedDate),
          }));
                if (pendingLoans.value.length > 0) {               
                    pendingLoansLastRequestDate.value = pendingLoans.value[0].RequestedDate;
                } else {

                    pendingLoansLastRequestDate.value = null;
                }
            } else {
                console.error("No se encontró ningun registro.");
            }
        } catch (error) {
            console.error(error.message);
        }
    };
    
    const userList = () => { router.push({ name: "listUsers" }); };

    const userProfile = (rowData) => {
        router.push({
            name: "updateUser",
            params: {
                id: rowData.data.PersonId
            },
            props: true,
        });
    };

    const loanList = () => {
        router.push({
            name: "loanRequestList"
        });
    };

    const loanRequest = (rowData) => {
        router.push({
            name: "updateLoanRequest",
            params: {
                id: rowData.data.LoanRequestId
            },
            props: true,
        });
    };

    const savingsList = () => {
        router.push({
            name: "savingsRequestList"
        });
    };

    const savingsRequest = (rowData) => {
        router.push({
            name: "updateSavingRequest",
            params: {
                id: rowData.data.SavingsRequestId
            },
            props: true,
        });
    }; 

    const loadingOverlay = async () => {
        isLoading.value = true;
        try {
            await Promise.all([
            fetchSavings(), 
            fetchUsers(), 
            fetchLoans()
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
    <div class="grid">
        <loading v-model:active="isLoading" :lock-scroll="true" />
        <toast-component />
        <div class="col-12 lg:col-6 xl:col-4">
            <div class="card mb-0">
                <div class="flex justify-content-between mb-3">
                    <div>
                        <span class="block text-500 font-medium mb-3">Asociados Activos</span>
                        <div class="text-900 font-medium text-xl">{{activeUsersCount}}</div>
                    </div>
                    <div class="flex align-items-center justify-content-center bg-blue-100 border-round"
                        style="width: 2.5rem; height: 2.5rem">
                        <i class="pi pi-user text-blue-500 text-xl"></i>
                    </div>
                </div>
            </div>
        </div>

        <div class="col-12 lg:col-6 xl:col-4">
            <div class="card mb-0">
                <div class="flex justify-content-between mb-3">
                    <div>
                        <span class="block text-500 font-medium mb-3">Solicitudes de Préstamos Activas</span>
                        <div class="text-900 font-medium text-xl">{{activeLoansCount}}</div>
                    </div>
                    <div class="flex align-items-center justify-content-center bg-orange-100 border-round"
                        style="width: 2.5rem; height: 2.5rem">
                        <i class="pi pi-credit-card text-orange-500 text-xl"></i>
                    </div>
                </div>
            </div>
        </div>

        <div class="col-12 lg:col-6 xl:col-4">
            <div class="card mb-0">
                <div class="flex justify-content-between mb-3">
                    <div>
                        <span class="block text-500 font-medium mb-3">Solicitudes de Ahorros Activas</span>
                        <div class="text-900 font-medium text-xl">{{activeSavingsCount}}</div>
                    </div>
                    <div class="flex align-items-center justify-content-center bg-orange-100 border-round"
                        style="width: 2.5rem; height: 2.5rem">
                        <i class="pi pi-money-bill text-cyan-500 text-xl"></i>
                    </div>
                </div>
            </div>
        </div>

        <panel :header="pendingUsersCount + ' solicitudes de aplicantes pendientes'"  toggleable class="panel col-12 xl:col-6" v-if="pendingUsersCount > 0">
            <div>              
                <h5>Día de la última solicitud <label style="float: right;">{{pendingUsersLastRequestDate}}</label>
                </h5>
                <div class="list">
                    <data-table :value="pendingUsers" :rows="5" paginator responsiveLayout="scroll">
                        <data-column field="FullName" style="width: 50%" sortable header="Nombre">
                        </data-column>
                        <data-column field="RequestedDate" style="width: 35%" sortable header="Fecha de solicitud">
                        </data-column>
                        <data-column style="width: 15%">
                            <template #header> </template>
                            <template #body="rowData">
                                <a type="button" class="text-xl" small label="" @click="userProfile(rowData)"><i
                                        class="pi pi-user-edit .text-gray-200 text-xl"></i></a>
                            </template>
                        </data-column>
                    </data-table>
                </div>
                
            </div>
            <h5>
                    <base-button class="action-buttons" :label="buttonsContent.SEEALL" @click="userList()"
                        :type="'button'" />
                </h5>
        </panel>

        <panel :header="pendingLoansCount + ' solicitudes de préstamos pendientes'" toggleable class="panel col-12 xl:col-6" v-if="pendingLoansCount > 0">
            <div>
                <h5>Día de la última solicitud <label
                        style="float: right;">{{pendingLoansLastRequestDate}}</label>
                </h5>
                <div class="list">
                    <data-table :value="pendingLoans" :rows="5" :paginator="true" responsiveLayout="scroll">
                        <data-column field="Name" style="width: 50%" sortable header="Nombre">
                        </data-column>
                        <data-column field="RequestedDate" style="width: 35%" sortable header="Fecha de solicitud">
                        </data-column>
                        <data-column style="width: 15%">
                            <template #header> </template>
                            <template #body="rowData">
                                <a type="button" class="text-xl" small label="" @click="loanRequest(rowData)"><i
                                        class="pi pi-file-edit .text-gray-200 text-xl"></i></a>
                            </template>
                        </data-column>
                    </data-table>
                </div>
                <h5>
                    <base-button class="action-buttons" :label="buttonsContent.SEEALL" @click="loanList()"
                        :type="'button'" />
                </h5>
            </div>
        </panel>

        <panel :header="pendingSavingsCount + ' solicitudes de ahorros pendientes'" toggleable class="panel col-12 xl:col-12" v-if="pendingSavingsCount > 0">
            <div>
                <h5>Día de la última solicitud <label style="float: right;">{{pendingSavingsLastRequestDate}}</label>
                </h5>
                <div class="list">
                    <data-table :value="pendingSavings" :rows="5" :paginator="true" responsiveLayout="scroll">
                        <data-column field="Name" style="width: 50%" sortable header="Nombre">
                        </data-column>
                        <data-column field="ApplicationDate" style="width: 50%" sortable header="Fecha de solicitud">
                        </data-column>
                        <data-column style="width: 15%">
                            <template #header> </template>
                            <template #body="rowData">
                                <a type="button" class="text-xl" small label="" @click="savingsRequest(rowData)"><i
                                        class="pi pi-file-edit .text-gray-200 text-xl"></i></a>
                            </template>
                        </data-column>
                     </data-table>
                   
                </div>
                <h5>
                    <base-button small class="action-buttons" :label="buttonsContent.SEEALL"
                        @click="savingsList()" :type="'button'" />
                </h5>
            </div>
            
        </panel>


    </div>
</template>

<style scoped="scoped">
    .list {
        margin-top: 1rem;
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
</style>