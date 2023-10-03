<script setup>
    import { ref, onMounted, computed, watch } from 'vue';
    import { useStore } from 'vuex';
    import { useRouter, useRoute } from 'vue-router';
    import { useToast } from 'primevue/usetoast';
    import { errorMessage } from '@/constants/ErrorMessages.js';
    import { confirmationMessage } from '@/constants/ConfirmationMessages.js';
    import Loading from 'vue-loading-overlay';
    import { useConfirm } from "primevue/useconfirm";
    import { localeContent, buttonsContent } from '@/constants/LabelContent.js';

    const store = useStore()
    const route = useRoute();
    const toast = useToast();
    const router = useRouter();
    const confirm = useConfirm();

    const isLoading= ref(false);
    const loadingUpdateButton = ref(false);
    const isInvalidBeneficiaryList =  ref([]);
    const PersonId = ref(route.params.id);

    const kinship = ref(['Padre', 'Madre', 'Conyugue', 'Hijo(a)', 'Hermano(a)', 'Otro']);

    const beneficiaryInfo = ref([{
        BeneficiaryName: null,
        BeneficiaryNumberId: null,
        BeneficiaryKin: null,
        BeneficiaryPercentage: null
    }]);


    const isRowEmpty = (row) => {
        return (
            !row.BeneficiaryName ||
            !row.BeneficiaryNumberId ||
            !row.BeneficiaryKin ||
            !row.BeneficiaryPercentage
        );
    };

    const addRow = () => {
        const lastIndex = beneficiaryInfo.value.length - 1;
        const previousRow = beneficiaryInfo.value[lastIndex];
        if (lastIndex >= 0 && isRowEmpty(previousRow)) {
            toast.add({
                severity: 'error',
                detail: errorMessage.BENEFICIARYISREQUERED,
                life: 3000
            });
            return;
        }
        beneficiaryInfo.value.push({
            BeneficiaryName: null,
            BeneficiaryNumberId: null,
            BeneficiaryKin: null,
            BeneficiaryPercentage: null,
        });
        isInvalidBeneficiaryList.value =  ref([]);
    };

    const removeRow = (index) => {
        beneficiaryInfo.value.splice(index, 1);
    };

    const showRemoveButton = computed(() => {
        return beneficiaryInfo.value.length > 1;
    });

    const fetchBeneficiaryData = async () => {
        await store.dispatch('beneficiaries/getBeneficiaries', {
            PersonId: PersonId.value
        });
        const beneficiariesData = store.getters["beneficiaries/getBeneficiaries"];
        try {
            beneficiaryInfo.value = beneficiariesData.map((beneficiary) => ({
                BeneficiaryName: beneficiary.BeneficiaryName,
                BeneficiaryNumberId: beneficiary.BeneficiaryNumberId,
                BeneficiaryKin: beneficiary.BeneficiaryKin,
                BeneficiaryPercentage: beneficiary.BeneficiaryPercentage
            }));
            isInvalidBeneficiaryList.value = calculateInvalidBeneficiaryList();      
        } catch (error) {
            toast.add({
                severity: 'error',
                detail: errorMessage.ERROR500,
                life: 3000
            });
        }
    };

    const userInfo = () => {
        router.push({
            name: "updateUser",
            params: {
                id: PersonId.value
            },
            props: true,
        });
    }

    const storeBeneficiary = async () => {
        await store.dispatch('beneficiaries/deleteAndInsertBeneficiaries', {
            PersonId: PersonId.value,
            beneficiaryInfo: beneficiaryInfo.value
        })
    }
    
    const isInvalidBeneficiaryCheck = () => {
  for (const beneficiary of beneficiaryInfo.value) {
    if (
      beneficiary.BeneficiaryName === null || beneficiary.BeneficiaryName.trim() === "" ||
      beneficiary.BeneficiaryNumberId === null || beneficiary.BeneficiaryNumberId.trim() === "" ||
      beneficiary.BeneficiaryKin === null || beneficiary.BeneficiaryKin.trim() === "" ||
      beneficiary.BeneficiaryPercentage === null || beneficiary.BeneficiaryPercentage === 0
    ) {
        isInvalidBeneficiaryList.value = calculateInvalidBeneficiaryList();    
      return false;
    }
  }
  return true;
    };

    const calculateInvalidBeneficiaryList = () => {
    return beneficiaryInfo.value.map((beneficiary) => {
        return (
        !beneficiary.BeneficiaryName || beneficiary.BeneficiaryName.trim() === '' ||
        !beneficiary.BeneficiaryNumberId || beneficiary.BeneficiaryNumberId.trim() === '' ||
        !beneficiary.BeneficiaryKin || beneficiary.BeneficiaryKin.trim() === '' ||
        beneficiary.BeneficiaryPercentage === null || beneficiary.BeneficiaryPercentage === 0
        );
    });
    };

    const getTotalPercentage = () => {
    return beneficiaryInfo.value.reduce((sum, beneficiary) => sum + (beneficiary.BeneficiaryPercentage || 0), 0);
    };

    const updateBeneficiaries = async () => {     
                try {
                    await storeBeneficiary();
                    toast.add({
                        severity: 'success',
                        detail: confirmationMessage.CHANGESSAVED,
                        life: 3000
                    });
                    await new Promise((resolve) => setTimeout(resolve, 1000));
                    router.push({
                        name: "updateUser",
                        params: { id: PersonId.value },
                        props: true,
                    });
                } catch (error) {
                    toast.add({
                        severity: 'error',
                        detail: errorMessage.ERROR500,
                        life: 3000
                    });
                }
            };

    const confirmUpdate = async (event) => {
        event.preventDefault();
        const isValidBeneficiaryInfo = isInvalidBeneficiaryCheck();
        const totalPercentage = getTotalPercentage();
        if (isValidBeneficiaryInfo) {
            if (totalPercentage !== 100) {
                toast.add({
                    severity: 'error',
                    detail: errorMessage.INCORRECTBENEFICIARYPERCENTAGE,
                    life: 3000
                });
                return;
            } else {
                try {
                    confirm.require({
                        message: confirmationMessage.CONFIRMUPDATE,
                        header: confirmationMessage.CONFIRMATIONHEADER,
                        acceptLabel: localeContent.YES,
                        icon: 'pi pi-exclamation-triangle',
                        accept: async () => {
                            try {
                                loadingUpdateButton.value = true;
                                await Promise.all([
                                    updateBeneficiaries()
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
                } catch (error) {
                    console.error(error);
                    toast.add({
                        severity: 'error',
                        detail: errorMessage.ERROR500,
                        life: 3000
                    });
                }
            }
        } else {
            toast.add({
                severity: 'error',
                detail: errorMessage.BENEFICIARYREQUIREDFIELDS,
                life: 3000
            });
        }
    };

    watch(() => beneficiaryInfo.value.length, (newLength) => {
        if (newLength === 0) {
            addRow();
        }
    });

    const loadingOverlay = async () => {
        isLoading.value = true;
        try {
            await Promise.all([
            fetchBeneficiaryData()
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
        <loading v-model:active="isLoading" :lock-scroll="true" />
        <toast-component />
        <div class="header">
            <base-button :label="'+'" style="width:3rem" class="buttons" @click="addRow" :type="'button'" />
        </div>
        <div class="body">
            <div class="form">
                <div v-for="(beneficiary, index) in beneficiaryInfo" :key="index" class="form-row">
                    <div class="p-float-label">
                        <input-text placeholder="Nombre completo" class="input-text form-margin-right"
                            :id="'beneficiary-name' + index" type="text" v-model="beneficiary.BeneficiaryName" 
                            :class="{'p-invalid': isInvalidBeneficiaryList[index] && (!beneficiary.BeneficiaryName || beneficiary.BeneficiaryName.trim() === '')}"/>
                        <label :for="'beneficiary-name' + index">Nombre completo</label>
                    </div>
                    <div class="p-float-label">
                        <input-text class="input-text form-margin-right" :id="'beneficiary-id' + index"
                            placeholder="Identificación" type="text" v-model="beneficiary.BeneficiaryNumberId" 
                            :class="{'p-invalid': isInvalidBeneficiaryList[index] && (!beneficiary.BeneficiaryNumberId || beneficiary.BeneficiaryNumberId.trim() === '')}"/>
                       
                        <label :for="'beneficiary-id' + index">Identificación</label>
                    </div>
                    <div class="p-float-label form-margin-right">
                    <drop-down class="input-text" id="beneficiary-kin" :class="{'p-invalid': isInvalidBeneficiaryList[index] && (!beneficiary.BeneficiaryKin || beneficiary.BeneficiaryKin.trim() === '')}" :options="kinship"  :emptyFilterMessage="localeContent.NOAVAILABLEOPTION" :emptyMessage="localeContent.NOAVAILABLEOPTION"
                    v-model="beneficiary.BeneficiaryKin" />
                <label for="beneficiary-kin">Parentesco</label>
                </div>      
                    <div class="p-float-label">
                        <input-number class="input-text form-margin-right" placeholder="Porcentaje"
                            :id="'beneficiary-percentage' + index" v-model="beneficiary.BeneficiaryPercentage" suffix="%"
                            :maxFractionDigits="2" :minFractionDigits="2"
                            :class="{'p-invalid': isInvalidBeneficiaryList[index] && (!beneficiary.BeneficiaryPercentage || beneficiary.BeneficiaryPercentage === 0)}"/>
                       
                        <label :for="'beneficiary-percentage' + index">Porcentaje</label>
                    </div>
                    <base-button :label="'-'" style="width:3rem" class="buttons" v-if="showRemoveButton"
                        @click="removeRow(index)" :type="'button'"></base-button>
                </div>
            </div>
        </div>
        <div class="actions">
            <base-button :label="buttonsContent.BACK" small @click="userInfo" :type="'button'" />
            <base-button :label="buttonsContent.UPDATE" small @click="confirmUpdate" :loading="loadingUpdateButton" :type="'submit'" />
        </div>
    </div>
</template>


<style scoped>
    .main {
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
        align-self: flex-end;
        margin-bottom: 4rem;
    }


    .body {
        overflow: scroll;
        min-height: 30vh;
        max-height: 30vh;
        margin-bottom: 3rem;
    }

    .form {
        display: flex;
        flex-direction: column;
        align-items: center;
        width: 100%;

    }

    .form-row {
        display: flex;
        justify-content: space-between;
        margin-bottom: 2rem;
        width: 100%;
        margin-top: 1.5rem;
    }

    .form-margin-right {
        margin-right: 1rem;
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

    .dropdown,
    .input-text {
        width: 200px;
    }

    .buttons {
        margin-right: 1rem;
    }
</style>