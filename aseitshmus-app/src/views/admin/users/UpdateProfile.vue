<script setup>
    import { ref, onMounted, computed, watch } from 'vue';
    import { useStore } from 'vuex';
    import { useRouter, useRoute } from 'vue-router';
    import {required } from '@vuelidate/validators'
    import useVuelidate from '@vuelidate/core'
    import { useToast} from 'primevue/usetoast';
    import { useConfirm } from "primevue/useconfirm";
    import { localeContent, buttonsContent } from '@/constants/LabelContent.js';
    import { getCurrentDate, parseDate } from '@/utils/DateUtils.js';
    import { confirmationMessage } from '@/constants/ConfirmationMessages.js';
    import { errorMessage } from '@/constants/ErrorMessages.js';
    import Loading from 'vue-loading-overlay';
       
    const toast = useToast();
    const route = useRoute();
    const store = useStore()
    const router = useRouter()
    const confirm = useConfirm();

    const isLoading= ref(false);
    const currentDate = parseDate(getCurrentDate()); 
    const PersonId = ref(route.params.id);
    const roleSelected = ref();
    const statusDB = ref();
    const loadingUpdateButton = ref(false);
    const loadingApproveButton = ref(false);
    const dataChangedFlag = ref(false);

    const rules = {
        PersonId: { required },
        NumberId: { required },
        FirstName: { required },
        LastName1: { required },   
        DateBirth: { required },
        RoleDescription: { required },
        EnrollmentDate: { required }, 
        WorkStartDate: { required },
        EmailAddress: { required }
    }

    const userInfo = ref({
        PersonId: null,
        NumberId: null,
        FirstName: null,
        LastName1: null,
        LastName2: null,
        DateBirth: null,
        RoleId: roleSelected,
        EnrollmentDate: null,
        WorkStartDate: null,
        EmailAddress: null
    });

    const originalUserInfo = ref({
        PersonId: null,
        NumberId: null,
        FirstName: null,
        LastName1: null,
        LastName2: null,
        DateBirth: null,
        RoleId: null,
        EnrollmentDate: null,
        WorkStartDate: null,
        EmailAddress: null
    });

    const roleList = ref([{
            name: 'Administrador',
            value: 1
        },
        {
            name: 'Presidente',
            value: 2
        },
        {
            name: 'Vice-Presidente',
            value: 3
        },
        {
            name: 'Asociado',
            value: 4
        }
    ]);

    const status = ref([{
            name: 'Activo',
            value: 1
        },
        {
            name: 'Inactivo',
            value: 0
        }
    ]);

    const email = ref({ emailAddress: null});

    watch(userInfo, (newUserInfo) => {
        const propertiesToCompare = ['PersonId', 'NumberId', 'FirstName', 'LastName1', 'LastName2', 'DateBirth',
        'RoleId', 'EnrollmentDate', 'WorkStartDate', 'EmailAddress'];
        const hasChanged = propertiesToCompare.some(key => newUserInfo[key] !== originalUserInfo.value[key]);
        dataChangedFlag.value = hasChanged;
    }, {
        deep: true
    });

    const manageUserStatus = async () => {
        await store.dispatch('users/patchUserStatus', {
            PersonId: PersonId.value
        })
    }

    const sendActivationEmail = async () => {
        try {
            email.value.emailAddress = userInfo.value.EmailAddress
             await store.dispatch('emails/sendUserActivationEmail',  {
                email: email.value
        }) 
            toast.add({
                severity: 'success',
                detail: confirmationMessage.EMAILSENTTOUSERSUCCESSFULLY,
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
    }

    const userResponse = computed(() => {
        return store.getters["users/getErrorResponse"];
    });

    const manageUser = async () => {
        await manageUserStatus();
        await fetchUserData();
        if (userResponse.value !== null) {
            toast.add({
                severity: 'error',
                detail: userResponse.value,
                life: 3000
            });
            store.commit('users/clearErrorResponse');
        } else {       
            if (statusDB.value === 0) {
                toast.add({
                    severity: 'warn',
                    detail: "Usuario ha sido desactivado.",
                    life: 3000
                });              
            } else {
                toast.add({
                    severity: 'warn',
                    detail: "Usuario ha sido activado.",
                    life: 3000
                });
            }
        }
    }

    const confirmApprove = async () => {
        const isValid = await validateForm();
        if (isValid) {
            confirm.require({
                message: '¿Está seguro que quiere activar el usuario?',
                header: confirmationMessage.CONFIRMATIONHEADER,
                icon: 'pi pi-exclamation-triangle',
                acceptLabel: localeContent.YES,
                accept: async () => {
                    try {
                        loadingApproveButton.value = true;
                        if (dataChangedFlag.value) {
                            await Promise.all([
                            submitData(),
                            manageUser()
                        ]);}
                        else{await Promise.all([
                            manageUser()
                        ]);}
                        dataChangedFlag.value = false
                    } catch (error) {
                        console.error(error);
                    } finally {
                        loadingApproveButton.value = false;
                    }
                    if (userResponse.value === null) {                      
                      await sendActivationEmail();
                    } else {
                        toast.add({
                            severity: 'error',
                            detail: userResponse.value,
                            life: 3000
                        });
                        store.commit('users/clearErrorResponse');
                    }                   
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

    const confirmReject = async () => {
        const isValid = await validateForm();
        if (isValid) {
            confirm.require({
                message: '¿Está seguro que quiere desactivar el usuario?',
                header: confirmationMessage.CONFIRMATIONHEADER,
                icon: 'pi pi-exclamation-triangle',
                acceptLabel: localeContent.YES,
                accept: async () => {
                    try {
                        loadingApproveButton.value = true;
                        if (dataChangedFlag.value) {await Promise.all([
                            submitData(),
                            manageUser()
                        ]);}
                        else{await Promise.all([
                            manageUser()
                        ]);}
                        dataChangedFlag.value = false
                    } catch (error) {
                        console.error(error);
                    } finally {
                        loadingApproveButton.value = false;
                    }
                },
                reject: () => {
                    toast.add({
                        severity: 'warn',
                        detail: 'Se canceló la transacción.',
                        life: 3000
                    });
                }
            });
        }
    };

    const userList = () => {
        router.push({
            name: "listUsers"
        });
    }

    const userDetails = () => {
        router.push({ name: "detailsUser" });
    }

    const updateBeneficiaries = () => {
        router.push({
            name: "updateBeneficiary",
            params: {
                id: PersonId.value
            },
            props: true,
        });
    };

    const storeUser = async () => {
        try {
        userInfo.value.EnrollmentDate = new Date(userInfo.value.EnrollmentDate);
        userInfo.value.DateBirth = new Date(userInfo.value.DateBirth);
        userInfo.value.WorkStartDate = new Date(userInfo.value.WorkStartDate);
        await store.dispatch('users/patchUser', {
            PersonId: PersonId.value,
            userInfo: userInfo.value
        });
        PersonId.value = userInfo.value.PersonId;
        const updatedRoute = {
            name: route.name,
            params: { id: userInfo.value.PersonId }
        };
        router.push(updatedRoute);
        } catch (error) {
        toast.add({
            severity: 'error',
            detail: errorMessage.ERROR500,
            life: 3000
                });
            }
    }

    const v$ = useVuelidate(rules, userInfo);
    
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
        await store.dispatch('users/getUserById', {
            rowId: PersonId.value
        });
        const userData = store.getters["users/getUsers"];
        originalUserInfo.value = store.getters["users/getUsers"];
        try {
            userInfo.value.PersonId = userData.PersonId;
            userInfo.value.NumberId = userData.NumberId;
            userInfo.value.FirstName = userData.FirstName;
            userInfo.value.LastName1 = userData.LastName1;
            userInfo.value.LastName2 = userData.LastName2;
            userInfo.value.RoleDescription = userData.RoleDescription;
            statusDB.value = userData.IsActive ? 1 : 0;
            userInfo.value.EmailAddress = userData.EmailAddress;            
            userInfo.value.DateBirth = new Date(Date.parse(userData.DateBirth)); 
            userInfo.value.WorkStartDate = new Date(Date.parse(userData.WorkStartDate)); 
            roleSelected.value = userData.RoleId;
        if (userData.EnrollmentDate) {
            userInfo.value.EnrollmentDate = new Date(Date.parse(userData.EnrollmentDate)); 
        } else {
            userInfo.value.EnrollmentDate = null;
        }
        } catch (error) {
            console.error(error);
        }
    };

    const submitData = async () => {
        try {
            await storeUser()
            if (userResponse.value !== null) {
                toast.add({
                    severity: 'error',
                    detail: userResponse.value,
                    life: 3000
                });
                store.commit('users/clearErrorResponse');
            } else {
                toast.add({
                    severity: 'success',
                    detail: confirmationMessage.CHANGESSAVED,
                    life: 3000
                });
            }
        } catch (error) {
            toast.add({
                severity: 'error',
                detail: errorMessage.ERROR500,
                life: 3000
            });
        }
        store.commit('users/clearErrorResponse');
    }

    const confirmUpdate = async () => {
        const isValid = await validateForm();
        if (isValid) {
            confirm.require({
                message: confirmationMessage.CONFIRMUPDATE,
                header: confirmationMessage.CONFIRMATIONHEADER,
                acceptLabel: localeContent.YES,
                icon: 'pi pi-exclamation-triangle',
                accept: async () => {
                    try {
                        loadingUpdateButton.value = true;
                        await Promise.all([
                            submitData()
                        ]);
                    } catch (error) {
                        console.error(error);
                    } finally {
                        loadingUpdateButton.value = false;
                    }
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
            fetchUserData()
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
    <confirm-dialog></confirm-dialog>    
    <div class="main">
        <toast-component />
        <div class="form">           
            <div class="header">
            <div class="form-row">
                <div class="p-float-label">
                    <input-text class="input-text form-margin-right" id="employee-code" type="text"
                        placeholder="Código de empleado" v-model="userInfo.PersonId"
                        :class="{'p-invalid': v$?.PersonId?.$error }" />
                    <label for="employee-code">Código de empleado</label>
                </div>
                <div class="p-float-label">
                    <input-text class="input-text" id="employee-code" type="text" placeholder="Identificación"
                        v-model="userInfo.NumberId" :class="{'p-invalid': v$?.BankAccount?.$error }" :maxlength="12"/>
                    <label for="employee-code">Identificación</label>
                </div>
                <div class="p-float-label form-margin-left">
                    <input-text placeholder="Nombre" class="input-text" id="employee-firstname"
                        type="text" v-model=" userInfo.FirstName" :class="{'p-invalid': v$?.FirstName?.$error }" />
                    <label for="employee-firstname">Nombre</label>
                </div>
            </div>
            <div class="form-row">           
                <div class="p-float-label">
                    <input-text placeholder="Apellido 1" class="dropdown form-margin-right" id="employee-lastname1" type="text"
                        v-model="userInfo.LastName1" :class="{'p-invalid': v$?.LastName1?.$error  }" />
                    <label for="employee-lastname1">Primer apellido</label>
                </div>
                <div class="p-float-label">
                    <input-text placeholder="Apellido 2" class="input-text" id="employee-lastname2"
                        type="text" v-model="userInfo.LastName2" :class="{'p-invalid': v$?.LastName2?.$error }" />
                    <label for="employee-lastname2">Segundo apellido</label>
                </div>
                <drop-down v-model="statusDB" id="status-list" :options="status" optionLabel="name" optionValue="value"
                :emptyFilterMessage="localeContent.NOAVAILABLEOPTION" :emptyMessage="localeContent.NOAVAILABLEOPTION"
                    placeholder="Estado" class="dropdown form-margin-left" disabled  />
            </div>
            <div class="form-row">
                <div class="p-float-label">
                    <date-picker v-model="userInfo.DateBirth" placeholder="Nacimiento" :maxDate="currentDate"
                        class="dropdown form-margin-right" dateFormat="dd-mm-yy" showIcon id="dob"
                        :class="{'p-invalid': v$?.DateBirth?.$error }" />
                    <label for="dob">Nacimiento</label>
                </div>
                <div class="p-float-label" >
                    <date-picker v-model="userInfo.WorkStartDate" :maxDate="currentDate" placeholder="Ingreso empresa" class="dropdown "
                        dateFormat="dd-mm-yy" showIcon :class="{'p-invalid': v$?.WorkStartDate?.$error}"
                        id="work-start-date" />
                    <label for="work-start-date">Ingreso empresa</label>
                </div>
                <div class="p-float-label form-margin-left">
                    <date-picker v-model="userInfo.EnrollmentDate" placeholder="Ingreso asociación"
                        class="dropdown" dateFormat="dd-mm-yy" showIcon
                        :class="{'p-invalid': v$?.EnrollmentDate?.$error}" id="enrollment-date" />
                    <label for="enrollment-date">Ingreso asociación</label>
                </div>
            </div>
            <div class="form-row" >
                <div class="p-float-label"  style="display: flex; align-content: space-between;">
                    <drop-down v-model="roleSelected" :options="roleList" optionLabel="name" optionValue="value"
                    :emptyFilterMessage="localeContent.NOAVAILABLEOPTION" :emptyMessage="localeContent.NOAVAILABLEOPTION"
                        id="role" placeholder="Rol" class="dropdown  form-margin-right" :class="{'p-invalid': v$?.roleSelected?.$error}" />
                    <label for="role">Rol</label>                   
                </div>
                <div class="p-float-label">
                    <input-text placeholder="Correo electrónico" class="input-text" id="email-address"
                        type="text" v-model="userInfo.EmailAddress" :class="{'p-invalid': v$?.EmailAddress?.$error }" />
                    <label for="email-address">Correo electrónico</label>
                </div>
                <div class="form-margin-right-last">
                    <base-button small :label="buttonsContent.MOREDETAILS" @click="userDetails" :type="'button'" style="width: 9rem;"/>       
                </div>
              </div>
            </div>  
        <div class="actions">
            <base-button class="action-buttons" small :label="buttonsContent.BACKTOTHELIST" @click="userList" :type="'button'" />
            <base-button class="action-buttons"  :label="buttonsContent.BENEFICIARIES" @click="updateBeneficiaries" :type="'button'" />
            <base-button class="action-buttons green" :loading="loadingApproveButton"  v-if="statusDB === 0" @click="confirmApprove" :label="buttonsContent.ACTIVATE"
                :type="'submit'" />
            <base-button class="action-buttons red" :loading="loadingApproveButton"  v-if="statusDB === 1" @click="confirmReject" :label="buttonsContent.INACTIVATE"
                :type="'submit'" />
            <base-button class="action-buttons"  :loading="loadingUpdateButton" :label="buttonsContent.UPDATE" @click="confirmUpdate" :type="'submit'" />
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
        margin-bottom: 2.4rem;
        margin-top: 2rem;
       
    }

    .form-margin-right {
        margin-right: 6rem;
    }

    .form-margin-right-last {
        margin-left: 18.49rem;

    }
    .form-margin-left {
        margin-left: 6rem;
    }

    .actions {
        margin-top: 2rem;
        display: flex;
        flex-direction: row;
        justify-content: space-between;     
    }

    .actions button {
        flex: 1;
        margin-right: 1rem;
    }

  
</style>