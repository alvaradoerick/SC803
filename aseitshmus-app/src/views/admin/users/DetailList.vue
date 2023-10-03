<script setup>
    import { ref, onMounted } from 'vue';
    import { useStore } from 'vuex';
    import { useRouter, useRoute } from 'vue-router';
    import { getDate } from '@/utils/DateUtils.js';
    import { statusName } from '@/utils/InputFieldsUtils.js';
    import Loading from 'vue-loading-overlay';
    import { buttonsContent} from '@/constants/LabelContent.js';

    const route = useRoute();
    const store = useStore()
    const router = useRouter()

    const isLoading= ref(false);
    const PersonId = ref(route.params.id);
    const personalData = ref({});


    const userInfo = () => {
        router.push({
            name: "updateUser",
            params: {
                id: PersonId.value
            },
            props: true,
        });
    }

    const fetchUserData = async () => {    
        try {
            await store.dispatch('users/getUserById', {
            rowId: PersonId.value
        });   
            personalData.value = store.getters["users/getUsers"];
        } catch (error) {
            console.error(error);
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

    onMounted(fetchUserData);

</script>

<template>
    <loading v-model:active="isLoading" :lock-scroll="true" />
    <div class="summary-container">
        <div class="summary-section">
            <strong class="summary-item">Código de empleado:</strong>
            <label class="summary-item">{{ PersonId}}</label>
        </div>
        <div class="summary-section">
            <strong class="summary-item">Número de identificación:</strong>
            <label class="summary-item">{{  personalData.NumberId }}</label>
        </div>
        <div class="summary-section">
            <strong class="summary-item">Nombre completo:</strong>
            <label class="summary-item"> {{ personalData.FirstName }} {{ personalData.LastName1 }} {{ personalData.LastName2 }}</label>
        </div>
        <div class="summary-section">
            <strong class="summary-item">Fecha de nacimiento:</strong>
            <label class="summary-item"> {{ getDate(personalData.DateBirth) }}</label>
        </div>
        <div class="summary-section">
            <strong class="summary-item">Fecha de ingreso:</strong>
            <label class="summary-item">{{getDate(personalData.WorkStartDate)}}</label>
        </div>
        <div class="summary-section">
            <strong class="summary-item">Fecha de asociación:</strong>
            <label class="summary-item">{{  getDate(personalData.EnrollmentDate) }}</label>
        </div>
        <div class="summary-section">
            <strong class="summary-item">Correo electrónico:</strong>
            <label class="summary-item">{{  personalData.EmailAddress }}</label>
        </div>
        <div class="summary-section">
            <strong class="summary-item">Número telefónico:</strong>
            <label class="summary-item">{{ personalData.PhoneNumber}}</label>
        </div>
        <div class="summary-section">
            <strong class="summary-item">Cuenta bancaria:</strong>
            <label class="summary-item">{{ personalData.BankAccount }}</label>
        </div>
        <div class="summary-section">
            <strong class="summary-item">Rol:</strong>
            <label class="summary-item">{{ personalData.RoleDescription }}</label>
        </div>
        <div class="summary-section last">
            <strong class="summary-item">Estado:</strong>
            <label class="summary-item">{{ statusName(personalData.IsActive) }}</label>
        </div>
        <div class="summary-footer">
            <base-button class="button"  :label="buttonsContent.BACK" @click="userInfo" :type="'button'" />
        </div>
    </div>
</template>

<style scoped="scoped">

.summary-container {
    display: flex;
    flex-direction: column;
    padding: 3rem;
    width: 100%;
    border: 1px solid #ccc;
    border-radius: 5px;
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