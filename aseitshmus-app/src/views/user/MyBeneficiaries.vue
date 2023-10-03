<script setup>
    import { ref, onMounted, computed } from 'vue';
    import { useStore } from 'vuex';
    import { useRouter } from 'vue-router';
    import { errorMessage } from '@/constants/ErrorMessages.js';
    import Loading from 'vue-loading-overlay';
    import { useToast } from 'primevue/usetoast';
    import { appendDecimal } from '@/utils/InputFieldsUtils.js'
    import { buttonsContent } from '@/constants/LabelContent.js';

    const store = useStore()
    const router = useRouter();
    const toast = useToast();

    const isLoading= ref(false);
    const PersonId =  computed(() => { return  store.getters["auth/getLoggedInUser"]; })

    const beneficiaryInfo = ref([{
        BeneficiaryName: null,
        BeneficiaryNumberId: null,
        BeneficiaryKin: null,
        BeneficiaryPercentage: null
    }]);


    const fetchBeneficiaryData = async () => {
        console.log(PersonId.value)
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
            name: "myProfile",
           
        });
    }

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
        <toast-component />
        <loading v-model:active="isLoading" :lock-scroll="true" />
        <data-table :value="beneficiaryInfo"  :rows="10" tableStyle="min-width: 60rem">
            <data-column field="BeneficiaryName" header="Nombre completo"  />
            <data-column field="BeneficiaryNumberId" header="Identificación" />
            <data-column field="BeneficiaryKin" header="Parentesco"  />
            <data-column field="BeneficiaryPercentage" header="Porcentaje"  >
                <template #body="slotProps">
                    {{ appendDecimal(slotProps.data.BeneficiaryPercentage) }}%
                </template></data-column>
        </data-table>
        <div class="actions">
            <base-button class="button" :label="buttonsContent.BACK" small @click="userInfo" :type="'button'" />
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

.actions {
    justify-content: flex-start;
    margin-top: 3rem;
}

.button {
    width: 12rem !important;
}

</style>