<script setup>
    import useVuelidate from '@vuelidate/core'
    import { required } from '@vuelidate/validators'
    import { useStore } from 'vuex'
    import { useRouter } from 'vue-router';
    import { ref, onMounted, computed, watch } from 'vue';
    import { useToast } from 'primevue/usetoast';
    import { api } from '@/api/AxiosInterceptors.js';
    import { errorMessage } from '@/constants/ErrorMessages.js';
    import { confirmationMessage } from '@/constants/ConfirmationMessages.js';
    import { showDecimal,appendDecimal } from '@/utils/InputFieldsUtils.js'
    import { localeContent, buttonsContent } from '@/constants/LabelContent.js';
    import Loading from 'vue-loading-overlay';
    import { moveCursorToBeginningForIBAN } from '@/utils/InputFieldsUtils.js';
    import { getCurrentDate, parseDate } from '@/utils/DateUtils.js';

    const { getters, dispatch } = useStore();
    const router = useRouter();
    const toast = useToast();
    const store = useStore();

    const currentDate = parseDate(getCurrentDate()); 
    const enteredBankAccount = ref(null)
    const loanTypesList = ref([]);
    const selectedLoanType = ref(null);
    const selectedBankAccount = ref(null);
    const BankAccountList = ref([]);
    const currentType = ref(null);
    const invalidTerm = ref(false);
    const invalidAmount = ref(false);
    const isLoading= ref(false);
    const loadingButton = ref(false);
    const otherBankAccount = ref(false);

    const toReturn = () => { router.push({ name: "myDashboard" }); }

    const userInfo = computed(() => getters['users/getUsers']);

    const LoanRequestedEmail = async () => {
        try {
            const userRequestedInformation = {
            EmailAddress: userInfo.value.EmailAddress, 
            FullName:  userInfo.value.FullName         
        };

        await dispatch('emails/loanRequestedEmail',  {
            userRequestedInformation: userRequestedInformation
        }) 
        toast.add({
                severity: 'success',
                detail: confirmationMessage.ADMINISTRATORHASBEENNOTIFIED,
                life: 3000
            });
        } catch (error) {
            console.error('Error sending email', error);
            toast.add({
                severity: 'error',
                detail: errorMessage.UNABLETOSENDEMAIL,
                life: 3000
            });
        }
    
    };

    const loanRequest = ref({
        LoansTypeId: selectedLoanType,
        AmountRequested: 0,
        Term: 0,
        BankAccount: null,
        RequestedDate: null
    })

    const updatedLoanData = computed(() => {
        return {
            ...loanRequest.value,
            BankAccount: selectedBankAccount.value === 'Otra cuenta' ? enteredBankAccount.value :
                selectedBankAccount.value,
        };
    });

    const fetchUserData = async () => {
        await dispatch('users/getById');
        let responseData = getters["users/getUsers"];
        selectedBankAccount.value = responseData.BankAccount ? responseData.BankAccount : null;
        if (responseData.BankAccount !== null && responseData.BankAccount !== undefined) {
            BankAccountList.value = [{
                    value: responseData.BankAccount,
                    label: responseData.BankAccount
                },
                {
                    value: 'Otra cuenta',
                    label: 'Otra cuenta'
                }
            ];
        } else {
            BankAccountList.value = [{
                value: 'Otra cuenta',
                label: 'Otra cuenta'
            }];
        }
    };

    const storeLoan = async () => {
        await dispatch('loanRequests/addLoanRequest', {
            loanRequest: updatedLoanData.value,
        });
    };

    const calculatedValues = computed(() => {
        return getters['loanRequests/getLoanCalculation'] || [];
    });

    const fetchActiveLoanTypes = async () => {
        try {
            const response = await api.get('LoansType/active-loans');
            loanTypesList.value = response.data;
            if (response.data.length > 0) {
                selectedLoanType.value = response.data[0].LoansTypeId;
            }
        } catch (error) {
            console.error(error)
            toast.add({
                severity: 'error',
                detail: errorMessage.ERROR500,
                life: 3000
            });
        }
    };

    const fetchCalculation = async () => {
        const loanData = {
            Amount: loanRequest.value.AmountRequested,
            Term: loanRequest.value.Term,
            LoansTypeId: selectedLoanType.value,
        };
        await dispatch('loanRequests/getLoanCalculation', {
            loanData: loanData,
        });
        //calculatedValues.value = getters['loanRequests/getLoanCalculation'];
    };
    

    const rules = {
        RequestedDate: { required },
        Term: { required },
        AmountRequested: { required },
        LoansTypeId: { required }
    }

    const requestResponse = computed(() => {
        return store.getters["loanRequests/getErrorResponse"];
    });


    watch(selectedLoanType, () => {
        fetchCalculation();
        currentType.value = loanTypesList.value.find(type => type.LoansTypeId === selectedLoanType.value);

    });

    watch(loanRequest.value, () => {
        fetchCalculation();
    });
    const v$ = useVuelidate(rules, loanRequest);

    const validateForm = async () => {  
        invalidTerm.value = false;
        invalidAmount.value = false;
        otherBankAccount.value = false;
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
        if (currentType.value.Term < loanRequest.value.Term) {
            invalidTerm.value = true;
            toast.add({
                severity: 'error',
                detail: 'El plazo ingresado no es válido.',
                life: 3000
            });
            return false;
        }  if (loanRequest.value.Term === 0) {
            invalidTerm.value = true;
            toast.add({
                severity: 'error',
                detail: 'Favor ingresar al menos un mes como plazo.',
                life: 3000
            });
            return false;
        }
         if ((calculatedValues.value[0].TotalAvailAmount < loanRequest.value.AmountRequested)) {
            invalidAmount.value = true;
            toast.add({
                severity: 'error',
                detail: 'El monto solicitado no es válido. Ingrese un monto menor o igual al monto total disponible.',
                life: 3000
            });
            return false;
        }  
        if (loanRequest.value.AmountRequested === 0.00 || loanRequest.value.AmountRequested === 'undefined' ) {
            invalidAmount.value = true;
            toast.add({
                severity: 'error',
                detail: 'El monto solicitado debe ser mayor a $0.00',
                life: 3000
            });
            return false;
        }
    if(updatedLoanData.value.BankAccount === null){
        otherBankAccount.value = true;
        toast.add({
                    severity: 'error',
                    detail: errorMessage.REQUIREDFIELDS,
                    life: 3000
                });
                return false
    }
        return true;
    }

    const onSend = async (event) => {
        event.preventDefault();
        const isValid = await validateForm();
        if (isValid) {
            try {
                try {
                    loadingButton.value = true;
                    await Promise.all([
                        storeLoan()
                    ]);
                    if (requestResponse.value !== null) {
                        toast.add({
                            severity: 'error',
                            detail: requestResponse.value,
                            life: 3000
                        });
                        store.commit('users/clearErrorResponse');
                    } else {
                        toast.add({
                            severity: 'success',
                            detail: confirmationMessage.REQUESTSENT,
                            life: 3000
                        });
                        await new Promise((resolve) => setTimeout(resolve, 1000));
                        LoanRequestedEmail();
                        toReturn();
                    }

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

    const loadingOverlay = async () => {
        isLoading.value = true;
        try {
            await Promise.all([
            fetchActiveLoanTypes(), 
            fetchUserData(), 
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
        <loading v-model:active="isLoading" :lock-scroll="true" />
        <toast-component />
        <div class="form">
            <div>
                <div class="form-row">
                    <div class="p-float-label">
                        <drop-down v-model="selectedLoanType" :options="loanTypesList" optionLabel="Description"
                        :emptyFilterMessage="localeContent.NOAVAILABLEOPTION" :emptyMessage="localeContent.NOAVAILABLEOPTION"
                            optionValue="LoansTypeId" placeholder="Tipo de crédito" class="dropdown form-margin-right"
                            id="loan-type"  :class="{'p-invalid': v$?.LoansTypeId?.$error}" />
                        <label for="loan-type">Tipo de crédito</label>
                    </div>
                    <div class="p-float-label">
                        <input-number placeholder="Monto a solicitar" class="input-text" id="amount" mode="currency"
                            currency="USD" locale="en-US" v-model="loanRequest.AmountRequested"
                            :class="{'p-invalid': v$?.AmountRequested?.$error || invalidAmount}" />
                        <label for="amount">Monto a solicitar</label>
                    </div>
                    <div class="p-float-label  form-margin-left">
                        <date-picker v-model="loanRequest.RequestedDate" placeholder="Fecha de solicitud de crédito"
                            class="dropdown" dateFormat="dd-mm-yy" showIcon id="requested-day" :minDate="currentDate"
                            :class="{'p-invalid': v$?.RequestedDate?.$error }" />
                        <label for="requested-day">Fecha de solicitud de crédito</label>
                    </div>
                </div>
                <div class="form-row">
                    <div class="p-float-label">
                        <input-number placeholder="Monto quincenal" class="input-text" id="term"
                            v-model="loanRequest.Term"
                            v-tooltip.focus="'De 1 mes hasta 5 años. Para compra de vehículo hasta por 10 años.'"
                            :class="{'p-invalid': v$?.Term?.$error || invalidTerm}" />
                        <label for="term">Plazo (meses)</label>
                    </div>
                    <div class="p-float-label" style="margin-left: 6rem;">
                        <drop-down v-model="selectedBankAccount" :options="BankAccountList" optionLabel="label"
                        :emptyFilterMessage="localeContent.NOAVAILABLEOPTION" :emptyMessage="localeContent.NOAVAILABLEOPTION"
                            optionValue="value" placeholder="Tipo de crédito" class="dropdown" id="bank-account" />
                        <label for="bank-account">Cuenta bancaria (IBAN)</label>
                    </div>
                    <div class="p-float-label form-margin-left" v-if="selectedBankAccount === 'Otra cuenta'">
                        <input-mask placeholder="Otra cuenta" class="input-text" id="other-account" type="text"
                            v-model="enteredBankAccount"  :class="{'p-invalid': otherBankAccount}" :maxlength="22" mask="CR99999999999999999" @click="moveCursorToBeginningForIBAN" />
                        <label for="other-account">Otra cuenta</label>
                    </div>
                </div>
                <div v-if="calculatedValues">
                    <data-table :value="calculatedValues" showGridlines>
                        <data-column header="Plazo máximo:" style="width: 200px">
                            <template #body="slotProps ">
                                <label>{{slotProps.data.Term}} mes(es)</label>
                            </template>
                        </data-column>
                        <data-column header="Monto disponible de ahorro:" style="width: 200px">
                            <template #body="slotProps ">
                                <label>{{showDecimal(slotProps.data.AvailEmployeeAmt)}}</label>
                            </template>
                        </data-column>
                        <data-column header="Monto disponible de aporte:" style="width: 200px">
                            <template #body="slotProps">
                                <label>
                                    {{showDecimal(slotProps.data.AvailEmployerAmt)}}
                                </label>
                            </template>
                        </data-column>
                        <data-column header="Total disponible:">
                            <template #body="slotProps">
                                <label>
                                    {{showDecimal(slotProps.data.TotalAvailAmount)}}
                                </label>
                            </template>
                        </data-column>
                        <data-column header="Tasa de interés:">
                            <template #body="slotProps">
                               {{appendDecimal(slotProps.data.Rate)}}%
                            </template>
                        </data-column>
                        <data-column header="Cuota quincenal:">
                            <template #body="slotProps">
                                <label>
                                    {{showDecimal(slotProps.data.BiweeklyFee)}}
                                </label>
                            </template>
                        </data-column>
                        <data-column header="Total a pagar:">
                            <template #body="slotProps">
                                <label>
                                   {{showDecimal(slotProps.data.TotalAmtToPay)}}
                                </label>
                            </template>
                        </data-column>
                    </data-table>
                </div>
            </div>
            <div class="actions">
                <base-button :label="buttonsContent.CANCEL" @click="toReturn" small :type="'button'" />
                <base-button :label="buttonsContent.SUBMIT" :loading="loadingButton" @click="onSend" small :type="'submit'" />
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