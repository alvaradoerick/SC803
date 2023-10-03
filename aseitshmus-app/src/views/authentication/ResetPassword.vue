<script setup>
    import useVuelidate from '@vuelidate/core'
    import { email, required } from '@vuelidate/validators'
    import { ref, computed, } from 'vue';
    import { useStore } from 'vuex';
    import { useToast } from 'primevue/usetoast';
    import { useRouter } from 'vue-router'
    import { errorMessage } from '@/constants/ErrorMessages.js';
    import { confirmationMessage } from '@/constants/ConfirmationMessages.js';
    import { buttonsContent } from '@/constants/LabelContent.js';

    const store = useStore();
    const toast = useToast();
    const router = useRouter();

    const passwordResponse = computed(() => {
        return store.getters["auth/getErrorResponse"];
    });

    const emailResponse = computed(() => {
        return store.getters["emails/getErrorResponse"];
    });

    
    const loading = ref(false);
    const isValiData = ref(false)

    const resetData = ref({ EmailAddress: null })

    const rules = {
        EmailAddress: {
            email,
            required
        }
    }

    const sendPasswordGeneratedEmail = async () => {
        try {   
            await store.dispatch('emails/generatePasswordEmail')
            if (emailResponse.value === null) {
                toast.add({
                    severity: 'success',
                    detail: confirmationMessage.EMAILSENTTOUSERSUCCESSFULLY,
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
        await store.dispatch('auth/resetPasswordUnauthenticated', {
            resetData: resetData.value,
        })
    }

    const loginPage = () => { router.push({ name: "login" }); }

    const v$ = useVuelidate(rules, resetData);

    const validateForm = async () => {
        const result = await v$.value.$validate();
        if (!result) {
            if (v$.value.$errors[0].$validator === 'required') {
                toast.add({
                    severity: 'error',
                    detail: 'Correo es requerido.',
                    life: 3000
                });
                return false
            } else if (v$.value.$errors[0].$validator === 'email') {
                toast.add({
                    severity: 'error',
                    detail: 'El formato del correo es incorrecto.',
                    life: 3000
                });
            }
            return false
        }
        return true;
    }

    const resetPassword = async (event) => {
        event.preventDefault();
        const isValid = await validateForm();
        if (isValid) {
            try {
                loading.value = true;
                try {
                    await Promise.all([
                        storeUser()
                    ]);
                    if (passwordResponse.value !== null) {
                    isValiData.value = true
                    toast.add({
                        severity: 'error',
                        detail: passwordResponse.value,
                        life: 3000
                    });
                    isValiData.value = false
                    store.commit('auth/clearErrorResponse');
                } else {
                    toast.add({
                        severity: 'success',
                        detail: "Su contraseña ha sido generada.",
                        life: 3000
                    });
                    sendPasswordGeneratedEmail();
                    await new Promise((resolve) => setTimeout(resolve, 2000));                   
                    router.push({ name: "login" });   
                    store.commit('auth/clearEmailToken');                  
                }
                } catch (error) {
                    console.error(error);
                } finally {
                    loading.value = false;
                }               
            } catch (error) {
                toast.add({
                    severity: 'error',
                    detail: errorMessage.ERROR500,
                    life: 3000
                });
            }
        }
    }

</script>

<template>
    <div class="password-container">
        <toast-component />
        <div class="form">
            <div style="margin-left: 40px;">
                <p>Por favor ingrese el correo eléctronico con el que está asociada su cuenta.</p>
                <p>Recibirá un correo con su nueva contraseña.</p>
            </div>
            <div class="center-container">
                <div class="container">
                    <div class="form-row">
                        <div class="p-float-label">
                            <input-text class="input-text " type="email" id="email-address"
                                v-model="resetData.EmailAddress" placeholder="Correo eléctronico"
                                :class="{'p-invalid': v$?.EmailAddress?.$error || isValiData }" />
                            <label for="email-address">Correo eléctronico</label>
                        </div>
                    </div>
                </div>
            </div>
            <div class="actions">
                <base-button :label="buttonsContent.CANCEL" type="login" @click="loginPage" />
                <base-button :label="buttonsContent.SUBMIT" type="submit" @click="resetPassword" :loading="loading" />
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

    .center-container {
        display: flex;
        justify-content: center;
        align-items: center;
        height: 28vh;
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

    .input-text {
        display: flex;
        width: 300px;
        align-items: center;
    }

    .actions {
        display: flex;
        flex: 1;
        align-items: center;
        justify-content: space-between;
        gap: 10%;
    }
</style>