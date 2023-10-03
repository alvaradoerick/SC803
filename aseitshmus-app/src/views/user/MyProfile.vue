<script setup>
    import { ref, onMounted, watch, computed } from 'vue';
    import { useStore } from 'vuex';
    import { useRouter } from 'vue-router';
    import { required } from '@vuelidate/validators'
    import useVuelidate from '@vuelidate/core'
    import { useToast } from 'primevue/usetoast';
    import { roles } from "@/constants/RolesConst.js";
    import { errorMessage } from "@/constants/ErrorMessages.js";
    import { confirmationMessage } from "@/constants/ConfirmationMessages.js";
    import { useConfirm } from "primevue/useconfirm";
    import Loading from 'vue-loading-overlay';
    import { localeContent, buttonsContent } from '@/constants/LabelContent.js';
    import { api }  from '@/api/AxiosInterceptors.js';
 
    const toast = useToast();
    const router = useRouter();
    const store = useStore();
    const confirm = useConfirm();

    const rules = {
        PhoneNumber: { required },
        BankAccount: { required },
        Address1: { required },        
        DistrictId: { required },
        PostalCode: { required }
    }

    const provincias = ref([]);
    const cantones = ref([]);
    const distritos = ref([]);
    const selectedProvincia = ref(null);
    const selectedCanton = ref(null);
    const selectedDistrito = ref(null);
    const PersonId =  store.getters["auth/getLoggedInUser"];
    const loadingButton = ref(false);
    const isLoading= ref(false);
    const personalData = ref({});
    const beneficiariesLabel = 'Beneficiarios';

    const fetchData = async (url, target) => {
        try {
            const response = await api.get(url);
            target.value = response.data;
        } catch (error) {
            console.error(error)
            toast.add({
                severity: 'error',
                detail: errorMessage.ERROR500,
                life: 3000
            });
        }
    };

    fetchData('location/provinces', provincias);

    watch(selectedProvincia, async (newValue) => {
        if (newValue) {
            cantones.value = [];
            distritos.value = [];
            fetchData(`location/province/${newValue}/cantons`, cantones);
        }
    });

    watch(selectedCanton, async (newValue) => {
        if (newValue) {
            distritos.value = [];
            fetchData(`/location/canton/${newValue}/districts`, distritos);
        }
    });

    const onProvinciaChange = () => {
        selectedCanton.value = null;
        selectedDistrito.value = null;
    };

    const onCantonChange = () => {
        selectedDistrito.value = null;
    };
 
    const personalInfo = ref({
        PhoneNumber: null,
        BankAccount: null,
        Address1: null,
        Address2: null,
        DistrictId: selectedDistrito,
        PostalCode: null
    });

    const storeUser = async () => {
        await store.dispatch('users/patchProfile', {
            personalInfo: personalInfo.value
        })
    }

    const role = computed(() => {
        return store.getters["auth/getRole"];
    });

    const homePage = () => {
        if (role.value == roles.ADMINISTRATOR) {
            router.push({ name: "dashboard" });
        } else {
            router.push({ name: "myDashboard" });
        }
    }

    const userResponse = computed(() => {
        return store.getters["users/getErrorResponse"];
    });

    const v$ = useVuelidate(rules, personalInfo);

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
        return true;
    }

    const fetchUserData = async () => {
    try {
        await store.dispatch('users/getById');
        personalData.value = await store.getters["users/getUsers"] || {};       
        personalInfo.value.PhoneNumber = personalData.value.PhoneNumber;
        personalInfo.value.BankAccount = personalData.value.BankAccount;
        personalInfo.value.Address1 = personalData.value.Address1;
        personalInfo.value.Address2 = personalData.value.Address2;
        selectedDistrito.value = personalData.value.DistrictId;
        personalInfo.value.PostalCode = personalData.value.PostalCode;
        api.get(`/location/district/${personalInfo.value.DistrictId}`)
            .then(response => {
                const responseData = response.data;
                selectedProvincia.value = responseData.ProvinceId;
                selectedCanton.value = responseData.CantonId;
                selectedDistrito.value = responseData.DistrictId;
            })
    } catch (error) {
        console.log(error)
        toast.add({
            severity: 'error',
            detail: errorMessage.ERROR500,
            life: 3000
        });
    }
    };

    const submitData = async () => {
        const isValid = await validateForm();
        if (isValid) {
            try {
                try {
                    loadingButton.value = true;
                await Promise.all([
                storeUser()
                ]);
                console.log(userResponse.value)
                if(userResponse.value === null || userResponse.value !== 'undefined'){
                    toast.add({
                    severity: 'success',
                    detail: confirmationMessage.CHANGESSAVED,
                    life: 3000
                });
                }
                else{
                 toast.add({
                    severity: 'error',
                    detail: errorMessage.ERROR500,
                    life: 3000
                });
                userResponse.value === null
                }
            } catch (error) {
                console.error(error);
            } finally {
                loadingButton.value = false;
            }
                
                await new Promise((resolve) => setTimeout(resolve, 1000));
                 homePage();
            } catch (error) {
                toast.add({
                    severity: 'error',
                    detail: errorMessage.ERROR500,
                    life: 3000
                });
            }
        }

    }

    const goToBeneficiaries = () => {
        router.push({ name: "myBeneficiaries" });
    }; 

    const confirmUpdate = async (event) => {
        event.preventDefault();
        const isValid = await validateForm();
        if (isValid) {
            confirm.require({
                message: confirmationMessage.CONFIRMUPDATE,
                header: confirmationMessage.CONFIRMATIONHEADER,
                acceptLabel: localeContent.YES,
                icon: 'pi pi-exclamation-triangle',             
                accept: async () => {
                    submitData();
                },
                reject: () => {                   
                    toast.add({
                        severity: 'warn',
                        detail: confirmationMessage.TRANSACTIONCANCELLED,
                        life: 3000
                    });
                }
            });
        }
    };

    const loadingOverlay = async () => {
        isLoading.value = true;
        try {
            await Promise.all([
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
        <confirm-dialog></confirm-dialog>  
        <toast-component />
        <loading v-model:active="isLoading" :lock-scroll="true" />
        <div class="form">
            <div class="form-column">
                <p><label><b>Código de empleado: </b></label>
                    {{ PersonId }}</p>
                <p><label><b>Número de identificación: </b> </label>
                    {{ personalData.NumberId }}</p>
                <p><label><b>Nombre completo: </b></label>
                    {{ personalData.FirstName }} {{ personalData.LastName1 }} {{ personalData.LastName2 }}</p>
            </div>
            <div class="header">
                <div class="form-row">
                    <div class="p-float-label">
                        <input-mask class="input-text form-margin-right" id="employee-phone" type="text"
                            placeholder="Número telefónico" v-model="personalInfo.PhoneNumber"
                            :class="{'p-invalid': (v$?.PhoneNumber?.$error) }" mask="9999-9999" />
                        <label for="employee-code">Número telefónico</label>
                    </div>
                    <div class="p-float-label">
                        <input-text class="input-text" id="employee-account" type="text" placeholder="Cuenta IBAN"
                            v-model="personalInfo.BankAccount" :class="{'p-invalid': (v$?.BankAccount?.$error) }" :maxlength="22" />
                        <label for="employee-code">Cuenta IBAN</label>
                    </div>
                </div>
                <div class="form-row">
                    <div class="p-float-label">
                        <input-text placeholder="Dirección 1" class="dropdown form-margin-right" id="employee-address1"
                            type="text" v-model="personalInfo.Address1"
                            :class="{'p-invalid': (v$?.Address1?.$error ) }" />
                        <label for="employee-code">Dirección 1</label>
                    </div>
                    <div class="p-float-label">
                        <input-text placeholder="Dirección 2" class="input-text" id="employee-address2" type="text"
                            v-model="personalInfo.Address2" />
                        <label for="employee-code">Dirección 2</label>
                    </div>
                </div>
                <div class="form-row">
                    <div class="p-float-label">
                        <drop-down class="dropdown form-margin-right" :options="provincias" v-model="selectedProvincia"
                            optionLabel="ProvinceName" optionValue="ProvinceId" @onChange="onProvinciaChange"
                            :emptyFilterMessage="localeContent.NOAVAILABLEOPTION" :emptyMessage="localeContent.NOAVAILABLEOPTION"
                            placeholder="Provincia" :class="{'p-invalid': (v$?.selectedProvincia?.$error) }" />
                        <label for="employee-code">Provincia</label>
                    </div>
                    <div class="p-float-label">
                        <drop-down class="dropdown" :options="cantones" v-model="selectedCanton"
                            optionLabel="CantonName" optionValue="CantonId" @onChange="onCantonChange"
                            :emptyFilterMessage="localeContent.NOAVAILABLEOPTION" :emptyMessage="localeContent.NOAVAILABLEOPTION"
                            placeholder="Cantón" :class="{'p-invalid': (v$?.selectedCanton?.$error) }" />
                        <label for="employee-code">Cantón</label>
                    </div>
                </div>
                <div class="form-row">
                    <div class="p-float-label">
                        <drop-down class="dropdown form-margin-right" :options="distritos" v-model="selectedDistrito"
                            optionLabel="DistrictName" optionValue="DistrictId" placeholder="Distrito"
                            :emptyFilterMessage="localeContent.NOAVAILABLEOPTION" :emptyMessage="localeContent.NOAVAILABLEOPTION"
                            :class="{'p-invalid': (v$?.selectedDistrito?.$error) }" />
                        <label for="employee-code">Distrito</label>
                    </div>
                    <div class="p-float-label">
                        <input-text class="input-text" id="employee-zip" type="text" v-model="personalInfo.PostalCode"
                            placeholder="Código postal" :class="{'p-invalid': (v$?.PostalCode?.$error) }" :maxlength="5" />
                        <label for="employee-code">Código postal</label>
                    </div>
                </div>

            </div>
            <div class="actions">
                <base-button class="action-buttons" small :label="beneficiariesLabel" @click="goToBeneficiaries" :type="'button'" />
                <base-button small :loading="loadingButton" :label="buttonsContent.UPDATE" @click="confirmUpdate" :type="'submit'" />
            </div>
        </div>
    </div>
</template>

<style scoped="scoped">
    .main {
        display: flex;
        justify-content: center;
        align-items: center;
        border: 1px solid #ebebeb;
        border-radius: 5px;
        margin: 1rem;
        flex-direction: row;
        padding: 2rem;
    }

    .form {
        display: flex;
        flex-direction: column;
        align-items: center;
        width: 100%;
    }

    .form-column {
        display: flex;
        flex-direction: column;
        min-height: 9vh;
        align-items: center;
    }

    .header {
        margin-top: 1.1rem;
    }

    .form-row {
        display: flex;
        justify-content: space-between;
        align-self: center;
        margin-top: 2rem;
        width: 100%;
    }

    .form-margin-right {
        margin-right: 6rem;
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