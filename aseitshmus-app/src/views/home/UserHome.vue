<script setup>
    import { ref, onMounted } from 'vue';
    import { useStore } from 'vuex'
    import { getDate } from '@/utils/DateUtils.js';
    import { getSubmissionStatus } from '@/utils/InputFieldsUtils.js';
    import { showDecimal, appendPercentageSign } from '@/utils/InputFieldsUtils.js'
    import Loading from 'vue-loading-overlay';

    const store = useStore();

    const pendingLoans =  ref([]);
    const pendingSavings =  ref([]);
    const contributionBalance =  ref([]);
    const loansBalance =  ref([]);
    const savingsBalance =  ref([]);
    const isLoading= ref(false);
    const savingsCount =  ref(null);
    const loansCount =  ref(null);

    const contributionBalanceCount =  ref(null);
    const loansBalanceCount =  ref(null);
    const savingsBalanceCount =  ref(null);

    let activeLoansCount;
    let activeSavingsCount;


    const fetchContributionBalance = async () => {
        try {
            const response = await store.dispatch('contributionBalances/getContributionByEmployee');
            if (response) {
                contributionBalance.value = Array.isArray(response) ? response : [response].filter(Boolean);
                contributionBalanceCount.value = contributionBalance.value.length;
                contributionBalance.value = contributionBalance.value.map(balance => ({
            ...balance,
            ApprovedDate: getDate(balance.ApprovedDate),   
            EmployeeContribution:  showDecimal(balance.EmployeeContribution),           
            EmployerContribution: showDecimal(balance.EmployerContribution),
            TotalContribution: showDecimal(balance.TotalContribution),
            TotalEmployeeContribution: showDecimal(balance.TotalEmployeeContribution),
            TotalEmployerContribution: showDecimal(balance.TotalEmployerContribution)                                 
            }));
            } else {
                console.error("No se encontró ningun registro.");
            }
        } catch (error) {
            console.error(error.message);
        }      
    };
 
    const fetchLoansBalance = async () => {
        try {
            const response = await store.dispatch('loansBalances/getLoansBalanceByEmployee');
    
            if (response) {
                loansBalance.value = Array.isArray(response) ? response : [response].filter(Boolean);
                    loansBalanceCount.value = loansBalance.value.length;
                        loansBalance.value = loansBalance.value.map(balance => ({
            ...balance,
            PaymentDate: getDate(balance.PaymentDate),
            TotalPayment: showDecimal(balance.TotalPayment),
            EndingBalance:showDecimal(balance.EndingBalance),
            ScheduledPayment: showDecimal(balance.ScheduledPayment),
            TotalPaid: showDecimal(balance.TotalPaid),
            Interest: appendPercentageSign(balance.Interest),
            }));
                    
            } else {
                console.error("No se encontró ningun registro.");
            }
        } catch (error) {
            console.error(error.message);
        }
    };

    const fetchSavingsBalance = async () => {
        try {
            const response = await store.dispatch('savingsBalances/getSavingsBalanceByEmployee');
            if (response) {
                savingsBalance.value = Array.isArray(response) ? response : [response].filter(Boolean);
                savingsBalanceCount.value = savingsBalance.value.length;
                savingsBalance.value = savingsBalance.value.map(balance => ({
            ...balance,
            SavingsStartDate: getDate(balance.SavingsStartDate),
            LastDeductedDate: getDate(balance.LastDeductedDate),
            LastAmountDeducted:showDecimal(balance.LastAmountDeducted),
            TotalSaved: showDecimal(balance.TotalSaved),
            SavingsRequestedAmount: showDecimal(balance.SavingsRequestedAmount),
            SavingsEndDate: getDate(balance.SavingsEndDate),
            }));
            } else {
                console.error("No se encontró ningun registro.");
            }
        } catch (error) {
            console.error(error.message);
        }
    };

    const fetchSavings = async () => {
        try {
            const response = await store.dispatch('savingsRequests/getSavingsByEmployee');
            if (response) {
                const savingsData = Array.isArray(response) ? response : [response].filter(Boolean);
                activeSavingsCount= savingsData.filter(savings => savings.IsActive == true).length;
                pendingSavings.value = savingsData.filter(savings => savings.IsActive == false && savings.ApprovedDate == null);
                savingsCount.value = pendingSavings.value.length;
                pendingSavings.value = pendingSavings.value.map(savings => ({
            ...savings,
            ApplicationDate: getDate(savings.ApplicationDate),
            IsApproved: getSubmissionStatus(savings.IsApproved),
            Amount: showDecimal(savings.Amount)
            }));
            } else {
                console.error("No se encontró ningun registro.");
            }
        } catch (error) {
            console.error(error.message);
        }
    };
    
    const fetchLoans = async () => {
        try {
            const response = await store.dispatch('loanRequests/getLoansByEmployee');
            if (response) {
                const loansData = Array.isArray(response) ? response : [response].filter(Boolean);
                activeLoansCount = loansData.filter(loan => loan.IsActive === true).length;
                pendingLoans.value = loansData.filter(loan => loan.IsActive === false && loan.ApprovedDate === null);
                loansCount.value = pendingLoans.value.length;
                pendingLoans.value = pendingLoans.value.map(loan => ({
            ...loan,
            RequestedDate: getDate(loan.RequestedDate),
            IsApproved: getSubmissionStatus(loan.IsApproved),
            AmountRequested:  showDecimal(loan.AmountRequested)
          }));
            } else {
                console.error("No se encontró ningun registro.");
            }
        } catch (error) {
            console.error(error.message);
        }
    };

    const loadingOverlay = async () => {
        isLoading.value = true;
        try {
            await Promise.all([
                fetchSavings(),
                fetchLoans(),
                fetchContributionBalance(),
                fetchLoansBalance(),
                fetchSavingsBalance()
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
        <div class="col-12 lg:col-4 xl:col-6" v-if="contributionBalanceCount > 0">
            <div class="card mb-0">
                <div class="flex justify-content-between mb-3">
                    <div>
                        <span class="block text-500 font-medium mb-3">Última Cuota del Aporte Obrero</span>
                        <div class="text-900 font-medium text-xl">{{contributionBalance[0].EmployeeContribution}}</div>
                    </div>
                    <div class="flex align-items-center justify-content-center bg-orange-100 border-round"
                        style="width: 2.5rem; height: 2.5rem">
                        <i class="pi pi-user text-orange-500 text-xl"></i>
                    </div>
                </div>
            </div>
        </div>
        <div class="col-12 lg:col-4 xl:col-6" v-if="contributionBalanceCount > 0">
            <div class="card mb-0">
                <div class="flex justify-content-between mb-3">
                    <div>
                        <span class="block text-500 font-medium mb-3">Última Cuota del Aporte Patronal</span>
                        <div class="text-900 font-medium text-xl">{{(contributionBalance[0].EmployerContribution).toLocaleString()}}</div>
                    </div>
                    <div class="flex align-items-center justify-content-center bg-orange-100 border-round"
                        style="width: 2.5rem; height: 2.5rem">
                        <i class="pi pi-building text-blue-500 text-xl"></i>
                    </div>
                </div>
            </div>
        </div>
        <div class="col-12 lg:col-4 xl:col-6" >
            <div class="card mb-0">
                <div class="flex justify-content-between mb-3">
                    <div>
                        <span class="block text-500 font-medium mb-3">Préstamos Activos</span>
                        <div class="text-900 font-medium text-xl">{{activeLoansCount}}</div>
                    </div>
                    <div class="flex align-items-center justify-content-center bg-orange-100 border-round"
                        style="width: 2.5rem; height: 2.5rem">
                        <i class="pi pi-credit-card text-pink-500 text-xl"></i>
                    </div>
                </div>
            </div>
        </div>
        <div class="col-12 lg:col-4 xl:col-6">
            <div class="card mb-0">
                <div class="flex justify-content-between mb-3">
                    <div>
                        <span class="block text-500 font-medium mb-3">Ahorros Extraordinarios Activos</span>
                        <div class="text-900 font-medium text-xl">{{activeSavingsCount}}</div>
                    </div>
                    <div class="flex align-items-center justify-content-center bg-orange-100 border-round"
                        style="width: 2.5rem; height: 2.5rem">
                        <i class="pi pi-money-bill text-cyan-500 text-xl"></i>
                    </div>
                </div>
            </div>
        </div>
        <panel header="Información Personal" toggleable class="panel col-12 xl:col-12" v-if="contributionBalanceCount > 0">
            <div>
                <div class="list">
                    <data-table :value="contributionBalance" responsiveLayout="scroll">
                        <data-column field="NumberId" style="width: 20%" header="Cédula">
                        </data-column>
                        <data-column field="ApprovedDate" style="width: 20%" header="Fecha de afiliación">
                        </data-column>
                        <data-column field="TotalEmployeeContribution" style="width: 20%" header="Total aporte obrero">
                        </data-column>
                        <data-column field="TotalEmployerContribution" style="width: 20%"
                            header="Total aporte patronal">
                        </data-column>
                        <data-column field="TotalContribution" style="width: 20%" header="Total ahorrado">
                        </data-column>
                       <!-- <data-column style="width: 15%">
                            <template #header> </template>
                            <template #body="rowData">
                                <base-button style="float: right;" class="action-buttons" label="Exportar"
                                    @click="userList(rowData)" :type="'button'" />
                            </template>
                        </data-column>-->
                    </data-table>
                </div>
            </div>
        </panel>

        <panel header="Préstamos Activos" toggleable class="panel col-12 xl:col-14"  v-if="activeLoansCount > 0">
            <div>
                <div class="list">
                    <template v-if="loansBalanceCount > 0">
                        <data-table :value="loansBalance" responsiveLayout="scroll">
                        <data-column field="LoanName" style="width:15%" header="Préstamo">
                        </data-column>
                        <data-column field="PaymentDate" style="width: 15%" header="Fecha de último pago">
                        </data-column>
                        <data-column field="ScheduledPayment" style="width: 15%" header="Cuota">
                        </data-column>
                        <data-column field="TotalPayment" style="width: 15%" header="Monto del último pago">
                        </data-column>
                        <data-column field="Interest" style="width: 15%" header="Tasa de interés">
                        </data-column>
                        <data-column field="TotalPaid" style="width: 15%" header="Total pago">
                        </data-column>
                        <data-column field="EndingBalance" style="width: 15%" header="Saldo">
                        </data-column>
                      <!--  <data-column style="width: 15%">
                            <template #header> </template>
                            <template #body="rowData">
                                <base-button style="float: right;" class="action-buttons" label="Exportar"
                                    @click="userList(rowData)" :type="'button'" />
                            </template> 
                        </data-column> -->
                    </data-table>
                    </template>
                    <template v-else>
                        <p>Aún no se ha hecho el primer rebajo.</p>
                    </template>
                </div>
            </div>
        </panel>

        <panel header="Ahorros Activos" toggleable class="panel col-12 xl:col-14" v-if="activeSavingsCount > 0">
            <div>
                <div class="list">
                    <template v-if="savingsBalanceCount > 0">
                        <data-table :value="savingsBalance" responsiveLayout="scroll">
                        <data-column field="SavingsName" style="width:15%" header="Ahorro">
                        </data-column>
                        <data-column field="SavingsStartDate" style="width: 15%" header="Fecha de apertura">
                        </data-column>
                        <data-column field="SavingsEndDate" style="width: 15%" header="Fecha de finalización">
                        </data-column>
                        <data-column field="SavingsRequestedAmount" style="width: 15%" header="Cuota quincenal">
                        </data-column>
                        <data-column field="LastAmountDeducted" style="width: 15%" header="Último monto deducido">
                        </data-column>
                        <data-column field="LastDeductedDate" style="width: 15%" header="Última fecha deducida">
                        </data-column>
                        <data-column field="TotalSaved" style="width: 15%" header="Total ahorrado">
                        </data-column>
                       <!-- <data-column style="width: 15%">
                            <template #header> </template>
                            <template #body="rowData">
                                <base-button style="float: right;" class="action-buttons" label="Exportar"
                                    @click="userList(rowData)" :type="'button'" />
                            </template>
                        </data-column> -->
                    </data-table>
                    </template>
                    <template v-else>
                        <p>Aún no se ha hecho el primer ahorro.</p>
                    </template>
                    
                </div>
            </div>
        </panel>

        <panel header="Solicitudes pendientes de préstamos" toggleable class="panel col-12 xl:col-12" v-if="loansCount > 0">
            <div >
                <div class="list">
                    <data-table :value="pendingLoans" :rows="5" paginator responsiveLayout="scroll">
                        <data-column field="LoanTypeName" style="width: 25%" sortable header="Préstamo">
                        </data-column>
                        <data-column field="AmountRequested" style="width: 15%" sortable header="Monto solicitado">
                        </data-column>
                        <data-column field="Term" style="width: 15%" sortable header="Plazo (meses)">
                        </data-column>
                        <data-column field="IsApproved" style="width: 15%" sortable header="Estado">
                        </data-column>
                        <data-column field="RequestedDate" style="width: 10%" sortable header="Fecha de solicitud">
                        </data-column>
                    </data-table>
                </div>
            </div>
        </panel>

        <panel header="Solicitudes pendientes de ahorros extraordinarios" toggleable class="panel panel col-12 xl:col-12" v-if="savingsCount > 0">
            <div>
                <div class="list">
                    <data-table :value="pendingSavings" :rows="5" :paginator="true" responsiveLayout="scroll">

                        <data-column field="SavingsTypeName" style="width: 30%" sortable header="Ahorro">
                        </data-column>
                        <data-column field="Amount" style="width: 20%" sortable header="Monto">
                        </data-column>
                        <data-column field="IsApproved" style="width: 20%" sortable header="Estado">
                        </data-column>
                        <data-column field="ApplicationDate" style="width: 20%" sortable header="Fecha de solicitud">
                        </data-column>
                    </data-table>
                </div>
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

    .panel{
margin-bottom: 2rem;
margin-left: 0.5rem;
    }
</style>