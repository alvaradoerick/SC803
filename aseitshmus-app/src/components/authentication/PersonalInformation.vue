<script setup>
    import countriesJson from '@/assets/countriesJson.json';
    import { ref, defineEmits, watch, defineProps, onMounted } from 'vue';
    import { getCurrentDate, parseDate } from '@/utils/DateUtils.js';
    import { moveCursorToBeginning } from '@/utils/InputFieldsUtils.js';
    import { localeContent } from '@/constants/LabelContent.js';

    const currentDate = parseDate(getCurrentDate()); 
    
    const personalInfo = ref({
        PersonId: null,
        NumberId: null,
        FirstName: null,
        LastName1: null,
        LastName2: null,
        Nationality: null,
        DateBirth: null,
    });

    const invalidPersonId = ref('Valid');
    const invalidNumberId = ref('Valid');
    const invalidFirstName = ref('Valid');
    const invalidLastName1 = ref('Valid');
    const invalidNationality = ref('Valid');
    const invalidDateBirth = ref('Valid');

    const countrySet = ref([]);
    const selectedType = ref('Cédula');
    const types = ref(['Cédula', 'DIMEX']);

    countrySet.value = countriesJson;

    const emits = defineEmits(['personal-info'])

    watch(personalInfo.value, (newValue) => {
        emits('personal-info', newValue)
    })

    const props = defineProps({
        personalInfoValidationResult: Array
    });

    watch(() => props.personalInfoValidationResult, () => {
        invalidPersonId.value = 'Valid'
        invalidNumberId.value = 'Valid'
        invalidFirstName.value = 'Valid'
        invalidLastName1.value = 'Valid'
        invalidNationality.value = 'Valid'
        invalidDateBirth.value = 'Valid'
        if (Array.isArray(props.personalInfoValidationResult)) {

            if (props.personalInfoValidationResult.includes('PersonId')) {
                invalidPersonId.value = 'Invalid'
            }
            if (props.personalInfoValidationResult.includes('NumberId')) {
                invalidNumberId.value = 'Invalid'
            }
            if (props.personalInfoValidationResult.includes('FirstName')) {
                invalidFirstName.value = 'Invalid'
            }
            if (props.personalInfoValidationResult.includes('LastName1')) {
                invalidLastName1.value = 'Invalid'
            }
            if (props.personalInfoValidationResult.includes('Nationality')) {
                invalidNationality.value = 'Invalid'
            }
            if (props.personalInfoValidationResult.includes('DateBirth')) {
                invalidDateBirth.value = 'Invalid'
            }
        }
    });

    onMounted(() => {
    moveCursorToBeginning();
});
</script>

<template>
    <div class="container">
        <div class="form"></div>
        <div class="form-row">
            <div class="p-float-label">
                <input-text class="input-text form-margin-right" id="employee-code" placeholder="Código de empleado"
                    type="text" v-model="personalInfo.PersonId" name="PersonId"
                    v-tooltip.focus="'Código localizado en Workday'"
                    :class="{'p-invalid': invalidPersonId==='Invalid'}" />
                <label for="employee-code">Código de empleado</label>
            </div>
            <div class="p-float-label">
                <drop-down class="input-text" id="type" :options="types"  :emptyFilterMessage="localeContent.NOAVAILABLEOPTION" :emptyMessage="localeContent.NOAVAILABLEOPTION"
                    v-model="selectedType"/>
                <label for="type">Tipo de identificación</label>
            </div>
        </div>  
        <div class="form-row">
            <div class="p-float-label">
                <input-mask v-if="selectedType === 'Cédula'" class="input-text form-margin-right" 
                id="mask-input" placeholder="99-9999-9999" @click="moveCursorToBeginning" v-model="personalInfo.NumberId" mask="99-9999-9999" :class="{'p-invalid': invalidNumberId==='Invalid'}" />
                <input-text v-else-if="selectedType === 'DIMEX'" class="input-text form-margin-right flex-auto"
                    placeholder="DIMEX" type="text" id="employee-dimex" v-model="personalInfo.NumberId"
                    :class="{'p-invalid': invalidNumberId==='Invalid'}" :maxlength="12" />
                <label for="type">Identificación</label>
            </div>
            <div class="p-float-label">
                <input-text class="input-text" placeholder="Nombre" id="employee-name" type="text"
                    v-model="personalInfo.FirstName" :class="{'p-invalid': invalidFirstName==='Invalid'}" />
                <label for="employee-name">Nombre</label>
            </div>
        </div>
        <div class="form-row">
            <div class="p-float-label">
                <input-text class="input-text form-margin-right" placeholder="Primer apellido" id="employee-lastname1"
                    type="text" v-model="personalInfo.LastName1" :class="{'p-invalid': invalidLastName1==='Invalid'}" />
                <label for="employee-lastname1">Primer apellido</label>
            </div>
            <div class="p-float-label">
                <input-text class="input-text" placeholder="Segundo apellido" id="employee-lastname2" type="text"
                    v-model="personalInfo.LastName2" />
                <label for="employee-lastname2">Segundo apellido</label>
            </div>
        </div>
        <div class="form-row">
            <div class="p-float-label">
                <drop-down class="dropdown form-margin-right" placeholder="Nacionalidad" filter="filter"
                    id="nationality" :options="countrySet" v-model="personalInfo.Nationality" optionLabel="name"  :emptyFilterMessage="localeContent.NOAVAILABLEOPTION" 
                    :emptyMessage="localeContent.NOAVAILABLEOPTION" optionValue="code" :class="{'p-invalid': invalidNationality==='Invalid'}"/>                 
                <label for="nationality">Nacionalidad</label>
            </div>
            <div class="p-float-label">
                <date-picker v-model="personalInfo.DateBirth" placeholder="Fecha de nacimiento" class="dropdown"
                    dateFormat="dd-mm-yy"  id="dob" showIcon :maxDate="currentDate" :class="{'p-invalid': invalidDateBirth==='Invalid'}" />
                <label for="dob">Fecha de nacimiento</label>
            </div>
        </div>
    </div>
</template>

<style scoped="scoped">
    .container {
        display: flex;
        flex-direction: column;
        align-items: center;
        width: 100%;
    }

    .form {
        margin-bottom: 1.2rem;
    }

    .form-row {
        display: flex;
        justify-content: space-between;
        margin-bottom: 2.5rem;
        width: 100%;
    }

    .form-margin-right {
        margin-right: 2rem;
    }
</style>