<script setup="setup">
    import { defineEmits, ref, watch, defineProps, onMounted } from 'vue'
    import { getCurrentDate, parseDate } from '@/utils/DateUtils.js'
    import { moveCursorToBeginning, moveCursorToBeginningForIBAN } from '@/utils/InputFieldsUtils.js';

    const currentDate = parseDate(getCurrentDate()); 

    const workInfo = ref({
        WorkStartDate: null,
        PhoneNumber: null,
        EmailAddress: null,
        BankAccount: null
    });

    const invalidWorkStartDate = ref('Valid');
    const invalidPhoneNumber = ref('Valid');
    const invalidEmailAddress = ref('Valid');
    const invalidBankAccount = ref('Valid');

    const emits = defineEmits(['work-info'])

    watch(workInfo.value, (newValue) => {
        emits('work-info', newValue)
    })

    const props = defineProps({
        workInfoValidationResult: Array
    });

    watch(() => props.workInfoValidationResult, () => {
        if (Array.isArray(props.workInfoValidationResult)) {
            invalidWorkStartDate.value = 'Valid'
            invalidPhoneNumber.value = 'Valid'
            invalidEmailAddress.value = 'Valid'
            invalidBankAccount.value = 'Valid'

            if (props.workInfoValidationResult.includes('WorkStartDate')) {
                invalidWorkStartDate.value = 'Invalid'
            }
            if (props.workInfoValidationResult.includes('PhoneNumber')) {
                invalidPhoneNumber.value = 'Invalid'
            }
            if (props.workInfoValidationResult.includes('EmailAddress')) {
                invalidEmailAddress.value = 'Invalid'
            }
            if (props.workInfoValidationResult.includes('BankAccount')) {
                invalidBankAccount.value = 'Invalid'
            }
        }
    });

    onMounted(() => {
    moveCursorToBeginning();
});

</script>

<template>
    <div class="container">
        <div style="margin-top: 2rem;">
            <div class="form-row">
                <div class="p-float-label">
                    <date-picker v-model="workInfo.WorkStartDate" placeholder="Fecha de ingreso a 3Pillar" id="start-work-date"
                        class="dropdown form-margin-right" showIcon :maxDate="currentDate"
                        :class="{'p-invalid': invalidWorkStartDate==='Invalid'}" />
                    <label for="start-work-date">Fecha de ingreso</label>
                </div>
                <div class="p-float-label">
                    <input-mask class="input-text" id="mask-input" type="tel" v-model="workInfo.PhoneNumber"
                        placeholder="Número telefónico" mask="9999-9999" @click="moveCursorToBeginning"
                        :class="{'p-invalid': invalidPhoneNumber==='Invalid'}" />
                    <label for="mask-input">Número telefónico</label>
                </div>
            </div>
            <div class="form-row">
                <div class="p-float-label">
                    <input-text class="input-text form-margin-right" id="employee-email" type="email"
                        v-model="workInfo.EmailAddress" placeholder="Correo electrónico"
                        :class="{'p-invalid': invalidEmailAddress==='Invalid'}" />
                    <label for="employee-email">Correo electrónico</label>
                </div>
                <div class="p-float-label">
                    <input-mask class="input-text" id="IBAN-input" type="text" v-model="workInfo.BankAccount"
                        placeholder="Cuenta IBAN" :class="{'p-invalid': invalidBankAccount==='Invalid'}" :maxlength="22" mask="CR99999999999999999" @click="moveCursorToBeginningForIBAN" />
                    <label for="IBAN-input">Cuenta IBAN</label>
                </div>
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
        margin-bottom: 8.2rem;
    }

    .form-row {
        display: flex;
        justify-content: space-between;
        margin-bottom: 3.5rem;
        width: 100%;
    }

    .form-margin-right {
        margin-right: 2rem;
    }
</style>