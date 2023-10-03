<script setup>
import { useStore } from 'vuex';
import { useRouter } from 'vue-router';
import { onMounted, computed, watch, ref } from 'vue';
import { useToast} from 'primevue/usetoast';
import NavMenu from '@/components/UI/NavMenu.vue';
import { setAuthToken, interceptors, resetAuthToken, setEmailHeader } from '@/api/AxiosInterceptors.js';
import { errorMessage } from '@/constants/ErrorMessages.js';

const store = useStore();
const router = useRouter();
const toast = useToast();

const unauthorizedCount = ref(0)
let hasShownToast = false;

const logout = () => {
  localStorage.removeItem('token');
  localStorage.removeItem('loggedInUser');
  localStorage.removeItem('role');

  resetAuthToken();
  store.dispatch('auth/logout');
  unauthorizedCount.value = 1;
}



const isAuthenticated = computed(() => store.getters['auth/isAuthenticated']);
const emailToken = computed(() => store.getters['auth/getEmailToken']);

onMounted(async() => {
  unauthorizedCount.value = 0;
  await store.dispatch('auth/fetchLoginData');
  setAuthToken();
  setEmailHeader(emailToken.value)
  interceptors(logout);
})


	
watch(emailToken, (newValue) => {
  setEmailHeader(newValue);
});


watch(unauthorizedCount, (newValue) => {
  if (newValue >= 1 && !hasShownToast) {
    toast.add({
      severity: 'warn',
      detail: errorMessage.SESSIONEXPIRED,
      life: 2000
    });
    hasShownToast = true; 
  } else if (newValue === 0) {
    hasShownToast = false; 
  }
});

watch(isAuthenticated, (newValue) => {
  if (!newValue) {
    return router.push('/');
  }
})



</script>

<template>
  <toast-component />
  <NavMenu class="menu"></NavMenu>
  <div class="container">
    <router-view></router-view>
  </div>
</template>

<style lang="scss">

@font-face {
  font-display: swap;
  font-family: 'Nunito';
  font-style: normal;
  font-weight: 400;
  src: url('assets/fonts/Nunito/Nunito-Regular.ttf') format('truetype');
}

@font-face {
  font-display: swap;
  font-family: 'Roboto';
  font-style: normal;
  font-weight: 100;
  src: url('assets/fonts/Roboto/Roboto-Thin.eot') format('embedded-opentype');
  src: url('assets/fonts/Roboto/Roboto-Thin.ttf') format('truetype');
  src: url('assets/fonts/Roboto/Roboto-Thin.woff') format('woff');
  src: url('assets/fonts/Roboto/Roboto-Thin.woff2') format('woff2');
}

.menu {
  display: flex;
  flex-direction: column;
  justify-content: flex-end;
  align-items: flex-end;
  padding-right: 10px;
  margin-bottom: 1rem;
}

.title {
  font-size: 1.5rem;
  font-weight: bold;
  margin-bottom: 1rem;
  text-transform: uppercase;
  font-family: nunito;
}

.dropdown,
.input-text {
  display: flex;
  width: 300px;
}

.links {
  color: #077CBC;
  display: flex;
  overflow: hidden;
  width: 20rem;
  text-align: center;
  flex-direction: column;
  align-items: center;
  justify-content: center;
}

.links:hover {
  color: #0b4666
}

.p-button-icon-only,
.p-datepicker-trigger {
  background-color: #077CBC !important;
  border: #077CBC !important;
}

.percentage-sign {
  position: absolute;
  top: 50%;
  right: 1rem;
  transform: translateY(-50%);
}

.p-tabview .p-tabview-nav li.p-highlight .p-tabview-nav-link {
  background: #ffffff;
  border-color: #077CBC !important;
  color: #077CBC !important;
}

.p-tabview-nav{
      margin-left: 3rem!important;
    }

@media (min-width: 768px) {
  .p-tabview-nav{
      margin-left: 3rem!important;
      width: 1150px!important;
    }
  }
</style>