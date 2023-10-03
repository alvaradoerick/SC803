<script setup="setup">
    import { ref, watch, computed, defineEmits, defineProps } from 'vue';
    import { useToast } from 'primevue/usetoast';
    import { errorMessage } from '@/constants/ErrorMessages';
    import { localeContent } from '@/constants/LabelContent.js';

    const toast = useToast();

    const selectedKinship = ref('Padre');
    const kinship = ref(['Padre', 'Madre', 'Conyugue', 'Hijo(a)', 'Hermano(a)', 'Otro']);
    const data = ref(true);

    const beneficiaryInfo = ref([{
        BeneficiaryName: null,
        BeneficiaryNumberId: null,
        BeneficiaryKin: selectedKinship,
        BeneficiaryPercentage: null
    }]);


    const props = defineProps({
        beneficiaryInfoValidationResult: Boolean
    });

    watch(() => props.beneficiaryInfoValidationResult, () => {
        data.value = props.beneficiaryInfoValidationResult;
    });

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
        data.value = true
    };

    const removeRow = (index) => {
        beneficiaryInfo.value.splice(index, 1);
    };

   const emits = defineEmits(['beneficiary-info'])

    const showRemoveButton = computed(() => {
        return beneficiaryInfo.value.length > 1;
    });

    watch(beneficiaryInfo.value, (newValue) => {
       
        emits('beneficiary-info', newValue)
    })

</script>


<template>
    <div class="container">
        <div class="sub-heading">
            <p>
            A continuación, designo a los beneficiarios de mis derechos en ASEISTHMUS quienes podrán hacer uso de ellos en caso de fallecimiento del suscrito.
        </p>
        </div>
        <div class="header">           
            <base-button :label="'+'" style="width:3rem" class="buttons" @click="addRow" :type="'button'" />
        </div>
        <div class="body">
            <div v-for="(beneficiary, index) in beneficiaryInfo" :key="index" class="form-row">
                <div class="p-float-label">
                    <input-text placeholder="Nombre completo" class="input-text form-margin-right" :id="'beneficiary-name-' + index"
                        type="text" v-model="beneficiary.BeneficiaryName"  :class="{'p-invalid': !data}" />
                    <label :for="'beneficiary-name-' + index">Nombre completo</label>
                </div>
                <div class="p-float-label">
                    <input-text class="input-text form-margin-right" id="beneficiary-id" placeholder="Identificación"
                        type="text" v-model="beneficiary.BeneficiaryNumberId" :class="{'p-invalid': !data}" :maxlength="12"/>
                    <label for="beneficiary-id">Identificación</label>
                </div>
                <div class="p-float-label form-margin-right">
                <drop-down class="input-text" id="beneficiary-kin" :class="{'p-invalid': !data}" :options="kinship"  :emptyFilterMessage="localeContent.NOAVAILABLEOPTION" :emptyMessage="localeContent.NOAVAILABLEOPTION"
                    v-model="selectedKinship" />
                <label for="beneficiary-kin">Parentesco</label>
                </div>      
                <div class="p-float-label">
                    <input-number class="input-text form-margin-right" placeholder="Porcentaje"
                        id="beneficiary-percentage" type="text" v-model="beneficiary.BeneficiaryPercentage" suffix="%" :maxFractionDigits="2" :minFractionDigits="2" :class="{'p-invalid': !data}"/>
                    <label for="beneficiary-percentage">Porcentaje</label>
                </div>
                <base-button :label="'-'" style="width:3rem" class="buttons" v-if="showRemoveButton" @click="removeRow(index)"
                    :type="'button'" />
            </div>
        </div>
    </div>
</template>

<style scoped>
    .sub-heading{
        display: flex;
        width: 90%;
    }
    .header {
        display: flex;
        width: 100%;
        justify-content: flex-end;
        margin-bottom: 1rem;
    }

    .body {
        margin-top: 2rem;
        overflow: scroll;
        min-height: 28vh;
        max-height: 28vh;
        padding-right: 2rem;
 
    }

    .container {
        display: flex;
        flex-direction: column;
        align-items: center;
        width: 100%;
        margin-bottom: 0.98em;
    }

    .form-row {
        display: flex;
        justify-content: space-between;
        margin-bottom: 2rem;
        width: 100%;
        margin-top: 1.5rem;
        padding-left: 2rem;
    }

    .form-margin-right {
        margin-right: 2rem;
    }

    .dropdown,
    .input-text {
        width: 170px;
    }
    .buttons {     
        margin-left: 1rem;
    }
</style>