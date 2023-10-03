<script setup>
    import { useStore } from 'vuex'
    import { useRouter, useRoute } from 'vue-router';    
    import { ref, onMounted, computed } from 'vue';
    import { useToast } from 'primevue/usetoast';
    import { required } from '@vuelidate/validators'
    import useVuelidate from '@vuelidate/core'
    import { getDate } from '@/utils/DateUtils.js';
    import { showDecimal } from '@/utils/InputFieldsUtils.js';
    import { confirmationMessage } from '@/constants/ConfirmationMessages.js';
    import { errorMessage } from '@/constants/ErrorMessages.js';
    import { roles } from "@/constants/RolesConst.js";
    import Loading from 'vue-loading-overlay';
    import { useConfirm } from "primevue/useconfirm";
    import Textarea from 'primevue/textarea';
    import { localeContent, buttonsContent} from '@/constants/LabelContent.js';


    const router = useRouter();
    const route = useRoute();
    const toast = useToast();
    const store = useStore();
    const confirm = useConfirm();

    const loanRequestId = ref(route.params.id);
    const isLoading= ref(false);
    const confirmMessage= ref(null);
    const loadingReviewerApproveButton = ref(false);
    const loadingReviewerRejectButton = ref(false);
    const loadingAdminApproveButton = ref(false);
    const loadingAdminRejectButton = ref(false);
    const loadingSendToReviewButton = ref(false);

    const userData = computed (() => {
        return store.getters["users/getUsers"];
    })

    const role = computed(() => {
        return store.getters["auth/getRole"];
    });

    const loanList = () => {
        router.push({ name: "loanRequestList" });
    };

    const loanState = ref({
        IsApproved: null,
        ApprovedDate: null,
        Comments: null
    });

    const loanResponse = computed(() => {
        return store.getters["savingsRequests/getErrorResponse"];
    });

   const reviewComments = ref({ 
        Comments: null  
    })

    const rules = {
        ApprovedDate: { required }
    };
    
    


    const fetchUser = async () => {
    if (loan.value.PersonId) {
        try {
            await store.dispatch('users/getUserById', {
                rowId: loan.value.PersonId
            });
        } catch (error) {
            console.error(error);
        }
    }
};

    const emailInformation = ref({ 
        FullName: null,
        EmailAddress: null
    });

    const sendApproveLoanEmail = async () => {
        try {
             await store.dispatch('emails/approveLoanEmail',  {
                emailInformation:{
                    ...emailInformation,
                    FullName: `${userData.value.FirstName} ${userData.value.LastName1} ${userData.value.LastName2}`,
                    EmailAddress: userData.value.EmailAddress
                }
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
    };

    const sendRejectLoanEmail = async () => {
        try {
             await store.dispatch('emails/rejectLoanEmail',  {
                emailInformation:{
                    ...emailInformation,
                    FullName: `${userData.value.FirstName} ${userData.value.LastName1} ${userData.value.LastName2}`,
                    EmailAddress: userData.value.EmailAddress
                }
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
    };

    const sendRequestLoanReviewEmail = async () => {
        try {
            const loanInformation = {
                LoanRequestId: loanRequestId.value, 
                FullName: loan.value.Name           
        };
        await store.dispatch('emails/requestLoanReviewEmail',  {
            loanInformation: loanInformation
        }) 
        toast.add({
                severity: 'success',
                detail: confirmationMessage.REVIEWERSHAVEBEENNOTIFIED,
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

    const sendRespondLoanReviewEmail = async (isReviewApproved) => {
        let status= "";
        if (isReviewApproved)
        {status = "aprobado"}
        else {status = "rechazado"}
        try {
            const loanInformation = {
                LoanRequestId: loanRequestId.value, 
                FullName: loan.value.Name,  
                Status:  status    
        };
        await store.dispatch('emails/respondLoanReviewEmail',  {
            loanInformation: loanInformation
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

    const storeLoan = async () => {
        await store.dispatch('loanRequests/updateLoan', {
            loanRequestId: loanRequestId.value,
            loanState: {
            ...loanState.value,  
            Comments: loan.value.Comments
        }
        })
    }

    const v$ = useVuelidate(rules, loanState);

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

    const loan = computed(() => {
        return store.getters["loanRequests/find"](loanRequestId.value) || {};
    })
    

    const fetchLoanData = async () => {
        await store.dispatch('loanRequests/getLoanById', {
            rowId: loanRequestId.value
        });
    };
   
    const sendReviewResponse = async (event) => {
        try {
            let isReviewApproved = null;
            if (event.target.innerText === buttonsContent.APPROVEREVIEW) {
                isReviewApproved = true;
                confirmMessage.value = '¿Está seguro que quiere aprobar la revisión?'
            } else if (event.target.innerText === buttonsContent.REJECTREVIEW) {
                isReviewApproved = false;
                confirmMessage.value = '¿Está seguro que quiere rechazar la revisión?'
            }
            confirm.require({
                message: confirmMessage.value,
                header: confirmationMessage.CONFIRMATIONHEADER,
                icon: 'pi pi-exclamation-triangle',
                acceptLabel: localeContent.YES,
                accept: async () => {
                    try {
                        if (!isReviewApproved) {
                            loadingReviewerRejectButton.value = true;
                        } else {
                            loadingReviewerApproveButton.value = true;
                        }
                        await Promise.all([

                            store.dispatch('loanRequests/respondLoanReview', {
                                rowId: loanRequestId.value,
                                reviewComments: {
                                    ...reviewComments,
                                    Comments: loan.value.Comments,
                                    IsReviewApproved: isReviewApproved
                                }
                            })
                        ]);
                        if (loanResponse.value === null) {
                            toast.add({
                                severity: 'success',
                                detail: confirmationMessage.CHANGESSAVED,
                                life: 3000
                            });
                            await new Promise((resolve) => setTimeout(resolve, 1000));
                            loanList()
                            sendRespondLoanReviewEmail(isReviewApproved)
                        }
else{
    toast.add({
                            severity: 'error',
                            detail: loanResponse.value,
                            life: 3000
                        });
                        loanResponse.value = null
                    }
                        
                      
                    } catch (error) {
                        console.error(error);
                    } finally {
                        loadingAdminRejectButton.value = false;
                        loadingAdminApproveButton.value = false;
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
            console.error(error)
            toast.add({
                severity: 'error',
                detail: errorMessage.ERROR500,
                life: 3000
            });
        }
    };

    const sendReviewRequest = async () => {
        try {
            try {
                loadingSendToReviewButton.value = true;
                await Promise.all([
                    store.dispatch('loanRequests/requestLoanReview', {
                        rowId: loanRequestId.value,
                        reviewComments: {
                            ...reviewComments,
                            Comments: loan.value.Comments,
                        }
                    })
                ]);
                if (loanResponse.value === null) {
                    toast.add({
                        severity: 'success',
                        detail: "Se ha enviado la solicitud de revisión.",
                        life: 3000
                    });
                    await sendRequestLoanReviewEmail();
                } else {
                    toast.add({
                        severity: 'error',
                        detail: loanResponse.value,
                        life: 3000
                    });
                }
            } catch (error) {
                console.error(error);
            } finally {
                loadingSendToReviewButton.value = false;
            }

            await new Promise((resolve) => setTimeout(resolve, 1000));
            loanList()
        } catch (error) {
            console.error(error)
            toast.add({
                severity: 'error',
                detail: errorMessage.ERROR500,
                life: 3000
            });
        }
    };

    const isApprovedDateNull = computed(() => loan.value.ApprovedDate === null);
    const isAdmin = computed(() => (role.value === roles.ADMINISTRATOR));
    const isReviewer = computed(() => (role.value === roles.PRESIDENT || role.value === roles.TREASURER));
    const adminActivitiesVisible = computed(() => (loan.value.IsApproved === null));
    const reviewerActivitiesVisible = computed(() => (loan.value.IsApproved === null) &&
        (loan.value.IsReviewRequired === true))


    const loanStatus = computed(() => {
        if (loan.value.IsApproved === true) {
            return 'Aprobado';
        } else if (loan.value.IsApproved === false) {
            return 'Rechazado';
        } else {
            return 'Pendiente';
        }
    });

    const reviewStatus = computed(() => {
        if (loan.value.IsReviewApproved === true && loan.value.ReviewRequiredDate !== null) {
            return 'Aprobado';
        } else if (loan.value.IsReviewRequired === true && loan.value.IsReviewApproved === null) {
            return 'Pendiente';
        } else if (loan.value.IsReviewApproved === false) {
            return 'Rechazado';
        } else {
            return 'N/A';
        }
    });

    const getReviewAction = computed(() => {
      if (loan.value.IsReviewRequired === true) {
        return 'Si';
      } else if (loan.value.IsReviewRequired === false || loan.value.IsReviewRequired === null) {
        return 'No';
      } else {
        return 'N/A';
      }
    });
    
    const submitData = async (event) => {
        event.preventDefault();
        const isValid = await validateForm();
        if (isValid) {
            try {
                if (event.target.innerText === buttonsContent.APPROVE) {
                    loanState.value.IsApproved = true;
                    confirmMessage.value = '¿Está seguro que quiere aprobar el préstamo?'
                } else if (event.target.innerText === buttonsContent.REJECT) {
                    loanState.value.IsApproved = false;
                    confirmMessage.value = '¿Está seguro que quiere rechazar el préstamo?'
                }
                confirm.require({
                    message: confirmMessage.value,
                    header: confirmationMessage.CONFIRMATIONHEADER,
                    icon: 'pi pi-exclamation-triangle',
                    acceptLabel: localeContent.YES,
                    accept: async () => {
                        try {
                            if (loanState.value.IsApproved === 0) {
                                loadingAdminRejectButton.value = true;
                            } else {
                                loadingAdminApproveButton.value = true;
                            }
                            await Promise.all([
                                storeLoan()
                            ]);

                        } catch (error) {
                            console.error(error);
                        } finally {
                            loadingAdminRejectButton.value = false;
                            loadingAdminApproveButton.value = false;
                        }
                        if (loanResponse.value === null) {
                            toast.add({
                                severity: 'success',
                                detail: confirmationMessage.CHANGESSAVED,
                                life: 3000
                            });
                            await new Promise((resolve) => setTimeout(resolve, 1000));
                            loanList();
                            if (loanState.value.IsApproved) {
                                await sendApproveLoanEmail();
                            }
                            else if (!loanState.value.IsApproved){
                                await sendRejectLoanEmail();
                            }
                            

                        } else {
                            toast.add({
                                severity: 'error',
                                detail: loanResponse.value,
                                life: 3000
                            });
                            store.commit('loanRequests/clearErrorResponse');
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
                console.log(error)
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
                fetchLoanData(),   
            ]);
        } catch (error) {
            console.error(error);
        } finally {
            isLoading.value = false;
        }
    }

    onMounted(async () => {
        await loadingOverlay(); 
        await fetchUser();      
    })

</script>

<template>
    <loading v-model:active="isLoading" :lock-scroll="true" />
    <confirm-dialog></confirm-dialog>    
    <toast-component />
    <div class="summary-container">
        <div class="summary-section">
            <strong class="summary-item">Código del préstamo:</strong>
            <label class="summary-item">{{ loanRequestId}}</label>
        </div>
        <div class="summary-section">
            <strong class="summary-item">Nombre completo:</strong>
            <label class="summary-item">{{  loan.Name }}</label>
        </div>
        <div class="summary-section">
            <strong class="summary-item">Tipo de préstamo:</strong>
            <label class="summary-item">{{  loan.LoanTypeName}}</label>
        </div>
        <div class="summary-section">
            <strong class="summary-item">Fecha de solicitud:</strong>
            <label class="summary-item">{{  getDate(loan.RequestedDate) }}</label>
        </div>
        <div class="summary-section" v-if="loan.AmountRequested !== undefined">
            <strong class="summary-item">Monto solicitado:</strong>
            <label class="summary-item">{{showDecimal(loan.AmountRequested)}}</label>
        </div>
        <div class="summary-section" >
            <strong class="summary-item">Plazo:</strong>
            <label class="summary-item">{{ loan.Term}} mes(es)</label>
        </div>
        <div class="summary-section">
            <strong class="summary-item">Estado del préstamo:</strong>
            <label class="summary-item">{{ loanStatus}}</label>
        </div>
        <div class="summary-section">
            <strong class="summary-item">Requiere revisión del presidente:</strong>
            <label class="summary-item">{{ getReviewAction}}</label>
        </div>
        <div class="summary-section">
            <strong class="summary-item">Fecha de revisión:</strong>
            <label class="summary-item">{{ getDate(loan.ReviewRequiredDate) }}</label>
        </div>
        <div class="summary-section">
            <strong class="summary-item">Estado de la revisión:</strong>
            <label class="summary-item">{{  reviewStatus}}</label>
        </div>
        <div class="summary-section">
            <strong class="summary-item">Fecha en la que se completó el préstamo:</strong>
            <date-picker v-if="isApprovedDateNull && isAdmin" v-model="loanState.ApprovedDate"
                placeholder="Fecha de conclusión" class="summary-item dropdown" dateFormat="dd-mm-yy" showIcon
                id="last-day" :class="{'p-invalid': v$?.ApprovedDate?.$error }" />
            <label v-else class="summary-item">{{  getDate(loan.ApprovedDate)}}</label>
        </div>

        <div class="summary-section last">
            <strong class="summary-item">Comentarios:</strong>
            <Textarea  id="comment" v-model="loan.Comments" rows="5" cols="45" class="summary-item" />   
         </div> 
        <div class="summary-footer">
            <!--Admin buttons-->
            <base-button :label="buttonsContent.BACK" @click="loanList" small :type="'button'" class="button" />
            <base-button :label="buttonsContent.APPROVE" class="green button" small @click="submitData" :type="'submit'"
                v-if=" isAdmin &&  adminActivitiesVisible" :loading="loadingAdminApproveButton"/>
            <base-button :label="buttonsContent.REJECT" class="red button" v-if="isAdmin && adminActivitiesVisible" small
                @click="submitData" :type="'submit'" :loading="loadingAdminRejectButton"/>
            <!--admin buttons to send for review-->
            <base-button :label="buttonsContent.REQUESTREVIEW" small @click="sendReviewRequest" :type="'button'"
                v-if="isAdmin && adminActivitiesVisible" class="button" :loading="loadingSendToReviewButton"/>
            <!--reviewer buttons-->
            <base-button :label="buttonsContent.APPROVEREVIEW" class="green button" small @click="sendReviewResponse"
                :type="'button'" v-if="isReviewer && reviewerActivitiesVisible" :loading="loadingReviewerApproveButton"/>
            <base-button :label="buttonsContent.REJECTREVIEW" class="red button" small @click="sendReviewResponse"
                :type="'button'" v-if="isReviewer && reviewerActivitiesVisible" :loading="loadingReviewerRejectButton"/>
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
    margin-bottom: 5rem;
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
    }
}

.button {
    width: 12rem !important;
}

</style>