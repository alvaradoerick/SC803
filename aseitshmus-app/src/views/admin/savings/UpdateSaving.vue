<script setup>
    import { useStore } from 'vuex'
    import { useRouter, useRoute } from 'vue-router';
    import { ref, onMounted, computed } from 'vue';
    import { useToast } from 'primevue/usetoast';
    import { required } from '@vuelidate/validators'
    import useVuelidate from '@vuelidate/core'
    import { getDate } from '@/utils/DateUtils.js';
    import { errorMessage } from '@/constants/ErrorMessages.js';
    import Loading from 'vue-loading-overlay';
    import { confirmationMessage } from '@/constants/ConfirmationMessages.js';
    import { useConfirm } from "primevue/useconfirm";
    import Textarea from 'primevue/textarea';
    import { showDecimal } from '@/utils/InputFieldsUtils.js';
    import { localeContent, buttonsContent } from '@/constants/LabelContent.js';

    const router = useRouter();
    const route = useRoute();
    const toast = useToast();
    const store = useStore();
    const confirm = useConfirm();

    const cancelSavingsLabel = 'Cancelar ahorro';
    const savingsRequestId = ref(route.params.id);
    const isLoading= ref(false);
    const confirmMessage= ref(null);
    const loadingApproveButton = ref(false);
    const loadingRejectButton = ref(false);
    const loadingCancelButton = ref(false);

    const approveUpdateLabel = computed(() => {
        if (savingsData.value.IsApproved !== true) {
            return 'Aprobar';
        } else {
            return 'Actualizar'; 
        }
    });

    const savingsData = computed(() => {
        return store.getters["savingsRequests/getSavings"] || {};
    })

    const savingsState = ref({ 
        IsApproved: null, 
        Comments: null
        
    })

    const cancelSavingsComments = ref({ 
        Comments: null  
    })

    const savingsResponse = computed(() => {
        return store.getters["savingsRequests/getErrorResponse"];
    });

    const displayApproveUpdateButton = computed(() => !savingsData.value.IsCanceled && savingsData.value.IsApproved !== false);
    const displayRejectButton = computed(() => savingsData.value.ApprovedDate === null);
    const displayCancelButton = computed(() => savingsData.value.IsActive === true && savingsData.value.IsApproved === true);

    const savingsList = () => {
        router.push({ name: "savingsRequestList" });
    }

    const rules = {
        Amount: { required }
    };

    const storeSaving = async () => {
    await store.dispatch('savingsRequests/updateSavings', {
        savingsRequestId: savingsRequestId.value,
        savingsState: {
            ...savingsState.value,  
            Amount: savingsData.value.Amount,
            Comments: savingsData.value.Comments

        }
    });
    };

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
        return true;
    }

    const fetchSavingsData = async () => {      
        try {
            await store.dispatch('savingsRequests/getSavingsById', {
            rowId: savingsRequestId.value
        });
        } catch (error) {
            toast.add({
                severity: 'error',
                detail: errorMessage.ERROR500,
                life: 3000
            });
        }
    };

    const cancelSavings = async () => {  
        try {
            console.log(savingsData.value.Comments)
            confirm.require({
                    message: confirmationMessage.CANCELSAVINGSMESSAGE,
                    header: confirmationMessage.CONFIRMATIONHEADER,
                    icon: 'pi pi-exclamation-triangle',
                    acceptLabel: localeContent.YES,
                    accept: async () => {
                        try {     
                            loadingCancelButton.value = true;                      
                            await Promise.all([
                             store.dispatch('savingsRequests/cancelSavings', {
                                rowId: savingsRequestId.value,
                                cancelSavingsComments: {
                                    ...cancelSavingsComments,
                                    Comments: savingsData.value.Comments,
                                }
                            })
                            ]);
                            if (savingsResponse.value === null ) {
                                toast.add({
                                severity: 'success',
                                detail: confirmationMessage.SAVINGSCANCELEDSUCCESSFULLY,
                                life: 3000
                            });
                            await new Promise((resolve) => setTimeout(resolve, 1000));
                            savingsList()
                        }
                            else{
                                toast.add({
                                severity: 'error',
                                detail: savingsResponse.value,
                                life: 3000
                            });
                            }
                            
                        } catch (error) {
                            console.error(error);
                        } finally {
                            loadingCancelButton.value = false;
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
        } catch (error) {
            toast.add({
                severity: 'error',
                detail: errorMessage.ERROR500,
                life: 3000
            });
        }
    };

    const submitData = async (event) => {
        event.preventDefault();
        const isValid = await validateForm();
        if (isValid) {
            try {
                if (event.target.innerText === buttonsContent.APPROVE) {
                    savingsState.value.IsApproved = true;
                    confirmMessage.value = confirmationMessage.APPROVESAVINGSMESSAGE
                } 
                else  if (event.target.innerText === buttonsContent.UPDATE) {
                    savingsState.value.IsApproved = true;
                    confirmMessage.value = confirmationMessage.UPDATESAVINGSMESSAGE
                }  else if (event.target.innerText === buttonsContent.REJECT) {
                    savingsState.value.IsApproved = false;
                    confirmMessage.value = confirmationMessage.REJECTSAVINGSMESSAGE
                }
                confirm.require({
                    message: confirmMessage.value,
                    header: confirmationMessage.CONFIRMATIONHEADER,
                    icon: 'pi pi-exclamation-triangle',
                    acceptLabel: localeContent.YES,
                    accept: async () => {
                        try {
                            if (savingsState.value.IsApproved === 0) {
                                loadingRejectButton.value = true;
                            } else {
                                loadingApproveButton.value = true;
                            }
                            await Promise.all([
                            storeSaving()
                            ]);
                            toast.add({
                                severity: 'success',
                                detail: confirmationMessage.CHANGESSAVED,
                                life: 3000
                            });
                            await new Promise((resolve) => setTimeout(resolve, 1000));
                            savingsList()
                        } catch (error) {
                            console.error(error);
                        } finally {
                            loadingRejectButton.value = false;
                            loadingApproveButton.value = false;
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
            } catch (error) {
                toast.add({
                    severity: 'error',
                    detail: errorMessage.ERROR500,
                    life: 3000
                });
            }
        }
    }


    const isApprovedState = computed(() => {
        if (savingsData.value.IsCanceled) {
            return 'Cancelado';
        } else if (savingsData.value.IsActive) {
            return 'Aprobado';
        } else if (savingsData.value.ApprovedDate === null) {
            return 'Pendiente';
        } else {
            return 'Rechazado';
        }
    });
 
    const loadingOverlay = async () => {
        isLoading.value = true;
        try {
            await Promise.all([
            fetchSavingsData()
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
    <toast-component />
    <div class="summary-container">
        <div class="summary-section">
            <strong class="summary-item">Código del ahorro:</strong>
            <label class="summary-item">{{ savingsRequestId }}</label>
        </div>
        <div class="summary-section">
            <strong class="summary-item">Nombre completo:</strong>
            <label class="summary-item">{{ savingsData.Name }}</label>
        </div>
        <div class="summary-section">
            <strong class="summary-item">Tipo de ahorro:</strong>
            <label class="summary-item">{{  savingsData.SavingsTypeName }}</label>
        </div>
        <div class="summary-section">
            <strong class="summary-item">Fecha de solicitud:</strong>
            <label class="summary-item">{{  getDate(savingsData.ApplicationDate) }}</label>
        </div>
        <div class="summary-section" v-if="savingsData.Amount !== undefined">
            <strong class="summary-item">Monto solicitado:</strong>
            <label  v-if="!displayApproveUpdateButton" class="summary-item">{{ showDecimal(savingsData.Amount)}}</label>
            <input-number v-else v-model="savingsData.Amount" mode="currency" 
            currency="USD" locale="en-US" placeholder="Monto solicitado" class="summary-item"
            :class="{'p-invalid': v$?.Amount?.$error }" />
        </div>
        <div class="summary-section">
            <strong class="summary-item">Estado del ahorro:</strong>
            <label class="summary-item">{{ isApprovedState }}</label>
        </div>
        <div class="summary-section">
            <strong class="summary-item">Fecha en la que se completó el ahorro:</strong>
            <label class="summary-item">{{ getDate(savingsData.ApprovedDate) }}</label>
        </div>
        <div class="summary-section last">
            <strong class="summary-item">Comentarios:</strong>
            <Textarea  id="comment" v-model="savingsData.Comments" rows="5" cols="45" class="summary-item"/>   
         </div>                
        <div class="summary-footer" v-if="savingsData !== null">
            <base-button :label="buttonsContent.BACK" small @click="savingsList" :type="'button'" class="button"/>
                    <base-button :label="approveUpdateLabel" v-if="displayApproveUpdateButton" class="green button" small @click="submitData" :type="'submit'" :loading="loadingApproveButton" />
                    <base-button :label="buttonsContent.REJECT"  class="red button" v-if="displayRejectButton" small @click="submitData"
                        :type="'submit'" :loading="loadingRejectButton" />
                    <base-button :label="cancelSavingsLabel"  class="red button" v-if="displayCancelButton" small @click="cancelSavings"
                        :type="'submit'" :loading="loadingCancelButton" />
        </div>
    </div>
</template>

<style scoped="scoped">

.summary-container {
    border: 1px solid #ccc;
    border-radius: 5px;
    display: flex;
    flex-direction: column;
    padding: 3rem;
    width: 100%;
}

.summary-section {
    display: flex;
    flex-direction: column;
    padding: 1rem;
    width: 100%;
}

.summary-item {
    width: 100%;
    min-width: 25rem;
}

.summary-section+.last {
    margin-bottom: 3rem;
}

.summary-footer {
    display: flex;
    flex-direction: column;
    width: 100%;
    align-items: center;
}

@media (min-width: 768px) {
    .summary-section {
        flex-direction: row;
        justify-content: space-between;
    }

    .summary-footer {

        flex-direction: row;
        justify-content: space-between;
        gap: 20px;
    }
}

.button {
    width: 12rem !important;
}

</style>