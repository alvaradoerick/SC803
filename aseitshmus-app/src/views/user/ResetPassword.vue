<script setup>
    import { ref, computed, watch, onMounted } from 'vue';
    import { useRouter } from 'vue-router';
    import { required, minLength, sameAs } from '@vuelidate/validators'
    import { useVuelidate } from '@vuelidate/core'
    import { useToast } from 'primevue/usetoast';
    import { useStore } from 'vuex';
    import { useConfirm } from "primevue/useconfirm";
    import { roles } from "../../constants/RolesConst.js";
    import { errorMessage } from "@/constants/ErrorMessages.js";
    import { confirmationMessage } from "@/constants/ConfirmationMessages.js";
    import { hashText } from '@/utils/SecurityUtils.js';
    import Password from 'primevue/password';
    import { localeContent, buttonsContent } from '@/constants/LabelContent.js';
    
    const toast = useToast();
    const router = useRouter();
    const store = useStore();
    const confirm = useConfirm();

    const password = ref(null);
    const confirmPassword = ref(null);
    const isInvalidData = ref(false);
    const loading = ref(false);
    const hashedPw = ref(null)
 
    const personId = computed(() => store.getters['auth/getLoggedInUser']);
    const userInfo = computed(() => store.getters['users/getUsers']);

    const rules = {
        password: { required, minLength: minLength(8) },
        confirmPassword: { required, sameAs: sameAs(hashedPw) }
    }
 
    watch(hashedPw, (newHashedPw) => {
        password.value =  hashText(newHashedPw);
    }); 
    
    const v$ = useVuelidate(rules, { password, confirmPassword });

    const passwordResponse = computed(() => {
        return store.getters["users/getErrorResponse"];
    });

    const validateForm = async () => {
    const result = await v$.value.$validate();
    const passwordValue = hashedPw.value; 
    const lowercaseRegex = /(?=.*[a-z])/;
    const uppercaseRegex = /(?=.*[A-Z])/;
    const digitRegex = /(?=.*[0-9])/;
    const lengthRegex = /(?=.{8,})/;

    const validRequirements =
    lowercaseRegex.test(passwordValue) &&
    uppercaseRegex.test(passwordValue) &&
    digitRegex.test(passwordValue) &&
    lengthRegex.test(passwordValue);


        if (!result) {
            if (v$.value.$errors[0].$validator === 'required') {
                toast.add({
                    severity: 'error',
                    detail: errorMessage.REQUIREDFIELDS,
                    life: 3000
                });
                return false
            } else if (v$?.value?.password?.$error) {
                toast.add({
                    severity: 'error',
                    detail: errorMessage.PASSWORDLENGTH,
                    life: 3000
                });
            } else if (v$?.value ?.confirmPassword?.$error) {
                toast.add({
                    severity: 'error',
                    detail: errorMessage.PASSWORDMISMATCH,
                    life: 3000
                });
            }
            return false
        }
        else if(!validRequirements){
            isInvalidData.value = true

            toast.add({
      severity: 'error',
      detail: 'La contraseña no cumple con los requerimientos.',
      life: 3000,
    });
    return false;
        }
        return true;
    }

    const storePassword = async () => {
        const resetData = {
            Password: password.value,
        };

        await store.dispatch('users/resetPasswordAuthenticated', {
            resetData: resetData,
        });
    };

    const role = computed(() => {
        return store.getters["auth/getRole"];
    });

    const email = ref({ emailAddress: null});

    const sendResetPasswordEmail = async () => {
        try {
            email.value.emailAddress = userInfo.value.EmailAddress
            await store.dispatch('emails/resetPasswordEmail', {
                email: email.value
            })
            toast.add({
                severity: 'success',
                detail: confirmationMessage.EMAILSENTTOREGISTEREDUSERSUCCESSFULLY,
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

    const onSend = async () => {
        isInvalidData.value = false
            try {
                await storePassword();
                if (passwordResponse.value !== null) {
                    isInvalidData.value = true
                    toast.add({
                        severity: 'error',
                        detail: passwordResponse.value,
                        life: 3000
                    });
                    store.commit('users/clearErrorResponse');
                } else {
                    toast.add({
                        severity: 'success',
                        detail: confirmationMessage.PASSWORDCHANGE,
                        life: 3000
                    });
                    await new Promise((resolve) => setTimeout(resolve, 2000));
                    homePage();
                }
            } catch (error) {
                toast.add({
                    severity: 'error',
                    detail: errorMessage.ERROR500,
                    life: 3000
                });
            }
    };
    
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
                        loading.value = true;
                        await Promise.all([
                            onSend()
                        ]);
                    } catch (error) {
                        console.error(error);
                    } finally {
                        loading.value = false;
                    }
                    if (passwordResponse.value === null) {
                        await sendResetPasswordEmail();
                    } else {
                        toast.add({
                            severity: 'error',
                            detail: passwordResponse.value,
                            life: 3000
                        });
                        store.commit('users/clearErrorResponse');
                    }
                },
                reject: () => {
                    password.value = '';
                    confirmPassword.value = '';
                    v$.value.$reset();
                    toast.add({
                        severity: 'warn',
                        detail: confirmationMessage.TRANSACTIONCANCELLED,
                        life: 3000
                    });
                }
            });
        }
    };

    const homePage = () => {
        if (role.value == roles.ADMINISTRATOR) {
            router.push({ name: "dashboard" });
        } else {
            router.push({ name: "myDashboard" });
        }
    }

   

    onMounted(async () => { 
        
        if (personId.value !== null) {
            await  store.dispatch('users/getById')
            } })


</script>

<template>
    <div class="password-container">
        <toast-component />
        <confirm-dialog></confirm-dialog>  
        <div class="form">
            <div class="container">
                <div class="form-row">
                    <div class="p-float-label">
                        <Password :class="{ 'p-invalid': (v$?.password?.$error || isInvalidData) }" type="password"
                            v-model="hashedPw" autocomplete="password" placeholder="Contraseña"  ref="passwordInputRef" toggleMask promptLabel="Elige una contraseña" 
                            weakLabel="Demasiado simple" mediumLabel="Complejidad promedio" strongLabel="Contraseña compleja" />                    
                        <label for="password">Contraseña</label>
                    </div>
                </div>
                <div class="form-row">
                    <div class="p-float-label">
                        <Password  id="reenter-password"
                            :class="{ 'p-invalid': (v$?.confirmPassword?.$error || isInvalidData) }" type="password"
                            v-model="confirmPassword" autocomplete="confirmPassword"
                            placeholder="Reingrese contraseña"  ref="confirmPasswordInputRef" toggleMask :feedback="false"/>
                        <label for="reenter-password">Reingrese contraseña</label>
                    </div>
                </div>
 
                <div class="form-row end">
                    <div>
                        <p class="mt-2"><strong>Requerimientos</strong></p>
                        <ul class="pl-2 ml-2 mt-0" style="line-height: 1.5">
                            <li>Al menos una minúscula</li>
                            <li>Al menos una mayúscula</li>
                            <li>Al menos un número</li>
                            <li>Mínimo 8 caracteres</li>
                            </ul>
                    </div>
                </div>
            </div>
            
            <div class="actions">
                <base-button small :loading="loading" :label="buttonsContent.UPDATE" @click="confirmUpdate" :type="'submit'" />
            </div>
        </div>

    </div>
</template>

<style scoped>
    .password-container {
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

    .container {
        display: flex;
        flex-direction: column;
        align-items: center;
        width: 100%;
        margin: auto;
    }

    .form-row {
        display: flex;
        flex-direction: column;
        align-items: center;
        margin-bottom: 2rem;
        width: 100%;
    }
    .form-row+.end {
        display: flex;
        flex-direction: column;
        align-items: flex-start;
        margin-bottom: 2rem;
        width: 100%;
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
    }
</style>