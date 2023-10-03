<script setup="setup">
    import PersonalInformation from '@/components/authentication/PersonalInformation.vue'
    import WorkInformation from '@/components/authentication/WorkInformation.vue'
    import AddressInformation from '@/components/authentication/AddressInformation.vue'
    import BeneficiaryInformation from '@/components/authentication/BeneficiaryInformation.vue'
    import { useStore } from 'vuex';
    import { ref, computed } from 'vue';
    import { useToast } from 'primevue/usetoast';
    import { required, email } from '@vuelidate/validators'
    import useVuelidate from '@vuelidate/core'
    import Stepper from '@/components/UI/Stepper.vue'
    import RegistrationConfirmation from '@/components/authentication/RegistrationConfirmation.vue';
    import { errorMessage } from '@/constants/ErrorMessages.js';
    import { confirmationMessage } from '@/constants/ConfirmationMessages.js';
    import { buttonsContent, localeContent } from '@/constants/LabelContent.js';
    import { useConfirm } from "primevue/useconfirm";


    const store = useStore()
    const toast = useToast();
    const confirm = useConfirm();

    const activeIndex = ref(1);
    const loading = ref(false);
    let beneficiaryInvalid = false;

    const personalInfo = ref({
        PersonId: null,
        NumberId: null,
        FirstName: null,
        LastName1: null,
        LastName2: null,
        Nationality: null,
        DateBirth: null
    });

    const workInfo = ref({
        WorkStartDate: null,
        PhoneNumber: null,
        EmailAddress: null,
        BankAccount: null
    });

    const emailResponse = computed(() => {
        return store.getters["emails/getErrorResponse"];
    });

    const addressInfo = ref({
        Address1: null,
        Address2: null,
        DistrictId: null,
        PostalCode: null
    });

    const beneficiaryInfo = ref([{
        BeneficiaryName: null,
        BeneficiaryNumberId: null,
        BeneficiaryKin: null,
        BeneficiaryPercentage: null
    }]);

    const emailInformation = ref({ EmailAddress: null })

    const getDataFromPersonalInfo = (value) => {
        personalInfo.value = {
            ...personalInfo.value,
            ...value
        }
    }

    const getDataFromWorkInfo = (value) => {
        workInfo.value = {
            ...workInfo.value,
            ...value
        }
    }

    const getDataFromAddressInfo = (value) => {
        addressInfo.value = {
            ...addressInfo.value,
            ...value
        }
    }

    const getDataFromBeneficiaryInfo = (value) => {
        beneficiaryInfo.value = value
    }

    const loginResponse = computed(() => {
        return store.getters["auth/getErrorResponse"];
    });


    const sendRegistrationEmail = async () => {
        try {
            emailInformation.value.EmailAddress = workInfo.value.EmailAddress
            await store.dispatch('emails/registerEmail', {
                emailInformation: emailInformation.value
            })
            if (emailResponse.value === null) {
                toast.add({
                    severity: 'success',
                    detail: confirmationMessage.EMAILSENTTOREGISTEREDUSERSUCCESSFULLY,
                    life: 3000
                });
            } else {
                toast.add({
                    severity: 'error',
                    detail: emailResponse.value,
                    life: 3000
                });
            }
        } catch (error) {
            console.error('Error sending email', error);
            toast.add({
                severity: 'error',
                detail: errorMessage.UNABLETOSENDEMAIL,
                life: 3000
            });
        }
    };

    const storeUser = async () => {
        try{      
            personalInfo.value.DateBirth = new Date(personalInfo.value.DateBirth);
            workInfo.value.WorkStartDate = new Date(workInfo.value.WorkStartDate);
            await store.dispatch('auth/register', {
            personalInfo: personalInfo.value,
            workInfo: workInfo.value,
            addressInfo: addressInfo.value,
            beneficiaryInfo: beneficiaryInfo.value,
        })
        sendRegistrationEmail()
    } 
        catch (error) {
        toast.add({
            severity: 'error',
            detail: errorMessage.ERROR500,
            life: 3000
                });
            }
    }

    const personalRules = {
        PersonId: {required},
        NumberId: { required },
        FirstName: { required },
        LastName1: { required },
        Nationality: { required },
        DateBirth: { required }
    };

    const vPersonal$ = useVuelidate(personalRules, personalInfo);

    const addressRules = {
        Address1: { required },        
        DistrictId: { required },
        PostalCode: { required }
    }

    const vAddress$ = useVuelidate(addressRules, addressInfo)

    const workRules = {
        WorkStartDate: { required },
        PhoneNumber: { required },
        EmailAddress: { required, email },
        BankAccount: { required }
    }

    const vWork$ = useVuelidate(workRules, workInfo)

    const beneficiaryRules = {
        BeneficiaryName: { required },
        BeneficiaryNumberId: { required },
        BeneficiaryKin: { required },
        BeneficiaryPercentage: { required }
    };

    const vBeneficiary$ = computed(() => {  
        return beneficiaryInfo.value.map(beneficiary => useVuelidate(beneficiaryRules, beneficiary).value);
    });


    const validateBeneficiaryInfo = async () => {
        const beneficiaryValidations = vBeneficiary$.value;
        const invalidBeneficiaries = beneficiaryValidations.filter(validation => validation.$invalid);
        return invalidBeneficiaries.length;
    };

    const checkSumOfPercentages = () => {
        const sum = beneficiaryInfo.value.reduce((acc, beneficiary) => {
            const percentage = Number(beneficiary.BeneficiaryPercentage);
            return acc + (isNaN(percentage) ? 0 : percentage);
        }, 0);
        return sum === 100;
    };

    
    const validateBeneficiaryPercentageSum = async () => {
        const isSumValid = checkSumOfPercentages();
        return isSumValid;
    };

    const validateForm = async () => {
        let hasNoErrors = false
        if (activeIndex.value === 1) {
            hasNoErrors = await vPersonal$?.value?.$validate();
        } else if (activeIndex.value === 2) {
            hasNoErrors = await vWork$?.value?.$validate();
        } else if (activeIndex.value === 3) {
            hasNoErrors = await vAddress$?.value?.$validate();
        } else if (activeIndex.value === 4) {
            const numberOfErrors =  await validateBeneficiaryInfo();
            hasNoErrors = numberOfErrors >= 1 ? false : true;
            beneficiaryInvalid = hasNoErrors;
           beneficiaryInfoValidationResult.value = beneficiaryInvalid;
        }
        if (!hasNoErrors) {
            if (vPersonal$?.value?.$error) {
                toast.add({
                    severity: 'error',
                    detail: 'Por favor revisar los campos de la información personal en rojo.',
                    life: 3000
                });
            } else if (vWork$?.value?.$error) {
                toast.add({
                    severity: 'error',
                    detail: 'Por favor revisar los campos de los datos empresariales en rojo.',
                    life: 3000
                });
            } 
            else if (vAddress$?.value?.$error) {
                toast.add({
                    severity: 'error',
                    detail: 'Por favor revisar los campos del domicilio en rojo.',
                    life: 3000
                });
            } else if (!beneficiaryInvalid) {
                toast.add({
                    severity: 'error',
                    detail: errorMessage.BENEFICIARYREQUIREDFIELDS,
                    life: 3000
                });
            }

            return false;
        }
        return true
    }

    const isValiData = ref(false)

    const submitData = async () => {    
                try {
                    try {
                        loading.value = true;
                        await Promise.all([
                            storeUser()
                        ]);
                    } catch (error) {
                        console.error(error);
                    } finally {
                        loading.value = false;
                    }
                    if (loginResponse.value !== null) {
                        isValiData.value = true
                        toast.add({
                            severity: 'error',
                            detail: loginResponse.value,
                            life: 3000
                        });
                        store.commit('auth/clearErrorResponse');
                    } else {
                        nextStep();
                    }
                } catch (error) {
                    toast.add({
                        severity: 'error',
                        detail: errorMessage.ERROR500,
                        life: 3000
                    });
                }
            } 

    const showTemplate = async (event) => {
        event.preventDefault();
        beneficiaryInvalid = true;
        beneficiaryInfoValidationResult.value = beneficiaryInvalid;
        const isValid = await validateForm();
        const isPercentageCorrect = await validateBeneficiaryPercentageSum();
        if (isValid) {
            if (!isPercentageCorrect) {
                toast.add({
                    severity: 'error',
                    detail: errorMessage.INCORRECTBENEFICIARYPERCENTAGE,
                    life: 3000
                });
            } else {

    confirm.require({
        group: 'templating',
        header: 'Términos y Condiciones',
        message: confirmationMessage.TERMSANDCONDITIONS,
        acceptLabel: localeContent.YES,
        accept: () => {
            submitData();
        },
        reject: () => {
            toast.add({ severity: 'warn', detail: 'Se canceló la transacción.', life: 3000 });
        }
    });
}
        }
    };

    const prevStep = () => {
        activeIndex.value -= 1
        personalInfoValidationResult.value = [];
        workInfoValidationResult.value = [];
        addressInfoValidationResult.value = [];
    }

    const nextStep = async () => {
        const isValid = await validateForm();
        const invalidPersonal = await getInvalidFields(vPersonal$?.value);
        const invalidWork = await getInvalidFields(vWork$?.value);
        const invalidAddress = await getInvalidFields(vAddress$?.value);
        if (isValid) {
            try {
                activeIndex.value += 1;
            } catch (error) {
                toast.add({
                    severity: 'error',
                    detail: errorMessage.ERROR500,
                    life: 3000
                });
            }
        } else {
            personalInfoValidationResult.value = invalidPersonal;
            workInfoValidationResult.value = invalidWork;
            addressInfoValidationResult.value = invalidAddress;
        }
    }

    const items = ref([{
            label: 'Información Personal',
            id: 1
        },
        {
            label: 'Datos Empresariales',
            id: 2
        },
        {
            label: 'Domicilio',
            id: 3
        },
        {
            label: 'Beneficiarios',
            id: 4
        },
        {
            label: confirmationMessage.CONFIRMATIONHEADER,
            id: 5
        }
    ]);

    const personalInfoValidationResult = ref(null);
    const workInfoValidationResult = ref(null);
    const addressInfoValidationResult = ref(null);
    const beneficiaryInfoValidationResult = ref(true);

    const getInvalidFields = async (validationData) => {
        const invalidFields = [];
        if (validationData) {
            for (const key in validationData) {
                if (validationData[key]?.$error) {
                    invalidFields.push(key);
                }
            }
        }
        return invalidFields;
    };


</script>

<template>
            <confirm-dialog group="templating" >
        <template #message="slotProps">
            <div class="p-dialog">
                <p >{{ slotProps.message.message }}</p>
            </div>
        </template>
    </confirm-dialog>
    <div class="register">
        <toast-component />

        <div class="header">
            <div class="steps">
                <stepper :items="items" :active="activeIndex" aria-label="Form Steps" />                
            </div>
            <div class="body">
                <keep-alive>
                    <PersonalInformation v-if="activeIndex === 1" @personal-info="getDataFromPersonalInfo"
                    :personalInfoValidationResult="personalInfoValidationResult"/>
                </keep-alive>
                <keep-alive>
                    <WorkInformation v-if="activeIndex === 2" @work-info="getDataFromWorkInfo"  :invalidFields="getInvalidFields(vWork$?.value)"
                    :workInfoValidationResult="workInfoValidationResult"/>
                </keep-alive>
                <keep-alive>
                    <AddressInformation v-if="activeIndex === 3" @address-info="getDataFromAddressInfo"  :invalidFields="getInvalidFields(vAddress$?.value)"
                    :addressInfoValidationResult="addressInfoValidationResult"/>
                </keep-alive>
                <keep-alive>
                    <BeneficiaryInformation v-if="activeIndex === 4" @beneficiary-info="getDataFromBeneficiaryInfo"
                    :beneficiaryInfoValidationResult="beneficiaryInfoValidationResult"/>
                </keep-alive>
                <keep-alive>
                    <RegistrationConfirmation v-if="activeIndex === 5" />
                </keep-alive>
            </div>
        </div>
        <div :class="[activeIndex === 1 ? 'single-button' : '', 'actions']">
            <base-button :label="buttonsContent.BACK" v-if="activeIndex > 1 && activeIndex < 5" @click="prevStep"
                :type="'button'" />
            <base-button :label="buttonsContent.CONTINUE" v-if="activeIndex < 4" @click="nextStep" :type="'button'" />
            <base-button :label="buttonsContent.SUBMIT" v-if="activeIndex === 4" :type="'submit'" @click="showTemplate" :loading="loading"/>
        </div>

</div>
</template>

<style scoped="scoped">
    .register {
        display: flex;
        flex-direction: column;
        border: 1px solid #ccc;
        border-radius: 5px;
        padding: 2rem;
        width: 100%;
    }

    .header {
        display: flex;
        flex-direction: column;
        align-items: center;
    }

    .single-button {
    margin-left: 24rem;
    justify-content: flex-start; 
  }
    .actions {
        display: flex;
        flex: 1;
        align-items: center;
        justify-content: space-between;
    }

    .steps {
        margin-bottom: 25px;
        width: 100%;
    }

    .p-dialog {
        max-width: 38rem!important;
        border-radius: 6px;
        box-shadow: none;
        border:  none;
    }
</style>
