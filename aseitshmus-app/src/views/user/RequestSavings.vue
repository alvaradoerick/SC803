<script setup>
    import useVuelidate from '@vuelidate/core'    
    import { required } from '@vuelidate/validators'    
    import { useStore } from 'vuex'
    import { useRouter } from 'vue-router';
    import { ref, onMounted, computed } from 'vue';    
    import { useToast } from 'primevue/usetoast';
    import { showDecimal } from '@/utils/InputFieldsUtils.js'
    import { getDate } from '@/utils/DateUtils.js';
    import { localeContent, buttonsContent  } from '@/constants/LabelContent.js';
    import { api } from '@/api/AxiosInterceptors.js';
    import { errorMessage } from '@/constants/ErrorMessages.js';
    import { confirmationMessage } from '@/constants/ConfirmationMessages.js';
    import Loading from 'vue-loading-overlay';

    const store = useStore();
    const router = useRouter();
    const toast = useToast();

    const savingsTypeList = ref([]);
    const selectedSavingsType = ref(null);
    const isLoading= ref(false);
    const invalidAmount = ref(false)
    const loadingButton = ref(false);

    const savingsData = ref({
        SavingsTypeId: selectedSavingsType,
        Amount: 0.00
    })

    const toReturn = () => {
        router.push({ name: "myDashboard" });
    }
    
    const storeSavings = async () => {
        await store.dispatch('savingsRequests/addSavingsRequest', {
            savingsData: savingsData.value,
        });
    };

    const fetchActiveSavings = async () => {
        try {
            const response = await api.get('SavingsType/active-savings');
            savingsTypeList.value = response.data;
            if (response.data.length > 0) {
                selectedSavingsType.value = response.data[0].SavingsTypeId;
            }
        } catch (error) {
            toast.add({
                severity: 'error',
                detail: error,
                life: 3000
            });
        }
    };

    const getSelectedSavingsType = () => {
        return savingsTypeList.value.find(type => type.SavingsTypeId === selectedSavingsType.value);
    };

    const rules = { 
        Amount: { required }, 
        SavingsTypeId: { required } 
    }

    const numberOfBiweeklies = computed(() => {

    const startDate = new Date(getSelectedSavingsType()?.StartDate);
    const endDate = new Date(getSelectedSavingsType()?.EndDate);

    const startDay = startDate.getDate();
    const startMonth = startDate.getMonth();
    const startYear = startDate.getFullYear();

    const endDay = endDate.getDate();
    const endMonth = endDate.getMonth();
    const endYear = endDate.getFullYear();

    const monthsWith31Days = new Set([0, 2, 4, 6, 7, 9, 11]);

    function isLeapYear(year) {
        return ((year % 4 == 0) && (year % 100 != 0)) || (year % 400 == 0);
    }

    const firstPayDayOfAnyMonth = 15;
    const secondPayDayOfTheEndMonth = endMonth === 1 ? (isLeapYear(endYear) ? 29 : 28) : (monthsWith31Days.has(endMonth) ? 31 : 30); //depends on the month and if it is Feb, also if it is a leap year

    const isStartAfterFirstPayDay = startDay > firstPayDayOfAnyMonth;

    const isEndBeforeFirstPayDay = endDay < firstPayDayOfAnyMonth;
    const isEndAfterFirstPayDayBeforeSecondPayDay = (endDay >= firstPayDayOfAnyMonth) && (endDay < secondPayDayOfTheEndMonth);

    const isStartEqualToEnd = (startYear === endYear) && (startMonth === endMonth) && (startDay === endDay);

    const fullMonths = (endYear - startYear) * 12 + (endMonth - startMonth);

    let partialMonths = 0;

    if (!isStartEqualToEnd) {
        if (isStartAfterFirstPayDay) {
            partialMonths = 1;
        } else {
            partialMonths = 2;
        }

        if (isEndBeforeFirstPayDay) {
            partialMonths = partialMonths - 2;
        } else if (isEndAfterFirstPayDayBeforeSecondPayDay) {
            partialMonths = partialMonths - 1;
        }
    }

    const totalBiweeklies = fullMonths * 2 + partialMonths;
    return totalBiweeklies;
    });

    const estimatedSavings = computed(() => {
    return numberOfBiweeklies.value * savingsData.value.Amount;
    });
	
    const v$ = useVuelidate(rules, savingsData);

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
        else if(savingsData.value.Amount === 0){
            toast.add({
                    severity: 'error',
                    detail: 'El monto solicitado debe ser mayor a $0.00',
                    life: 3000
                });
                invalidAmount.value = true;
            return false
        }
        return true;
    }

    const onSend = async (event) => {
        event.preventDefault();
        const isValid = await validateForm();
        if (isValid) {
            try {
                invalidAmount.value = false
                try {
                    loadingButton.value = true;
                await Promise.all([
                storeSavings()
                ]);
                toast.add({
                    severity: 'success',
                    detail: confirmationMessage.REQUESTSENT,
                    life: 3000
                });
                await new Promise((resolve) => setTimeout(resolve, 1000));
                toReturn();
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
            fetchActiveSavings(), 
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
            <div class="header">
                <div class="form-row">
                    <div class="p-float-label">
                        <drop-down v-model="selectedSavingsType" :options="savingsTypeList" optionLabel="Description"
                            optionValue="SavingsTypeId" placeholder="Ahorro" class="dropdown form-margin-right" :emptyFilterMessage="localeContent.NOAVAILABLEOPTION" 
                            :emptyMessage="localeContent.NOAVAILABLEOPTION" id="savings-type"   :class="{'p-invalid': (v$?.SavingsTypeId?.$error  )}"/>
                        <label for="savings-type">Ahorro</label>
                    </div>
                    <div class="p-float-label">
                        <input-number placeholder="Monto quincenal" class=" input-text " id="amount" mode="currency"
                            currency="USD" locale="en-US" v-model="savingsData.Amount"
                            :class="{'p-invalid': (v$?.Amount?.$error || invalidAmount )}" />
                        <label for="amount">Monto quincenal</label>
                    </div>
                </div>
                <div v-if="savingsTypeList.length > 0">
                    <data-table :value="[{ id: 1 }]" showGridlines :paginator="false">
                        <data-column field="id" header="Empieza" style="width: 200px">
                            <template #body="{}">
                                {{  getDate(getSelectedSavingsType()?.StartDate) }}
                            </template>
                        </data-column>
                        <data-column field="id" header="Finaliza" style="width: 200px">
                            <template #body="{}">
                                {{ getDate(getSelectedSavingsType()?.EndDate) }}
                            </template>
                        </data-column>
                        <data-column header="Monto aproximado al final del ahorro:">
                            <template #body="{}">
                               {{showDecimal(estimatedSavings)}}                            
                            </template>
                        </data-column>
                    </data-table>
                </div>
            </div>
            <div class="actions">
                <base-button :label="buttonsContent.CANCEL" @click="toReturn" small :type="'button'" />
                <base-button :label="buttonsContent.SUBMIT" @click="onSend" :loading="loadingButton" small :type="'submit'" />
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

    .upload-button {
        display: flex;
        background-color: #253e8b;
        border-color: #253e8b;
        overflow: hidden;
        width: 300px;
        color: white;
        text-align: center;
        flex-direction: column;
        align-items: center;
        justify-content: center;
    }


    .upload-button:hover,
    .upload-button:focus {
        box-shadow: 0 0 0 2px white, 0 0 0 3px skyblue;
        color: white;
        background-color: #3f569b !important;
    }
</style>