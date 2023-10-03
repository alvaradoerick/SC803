<script setup>
    import { useStore } from 'vuex'
    import { computed, onMounted, ref, watch } from 'vue';
    import TabView from 'primevue/tabview';
    import TabPanel from 'primevue/tabpanel';
    import Card from 'primevue/card';
    import { useToast } from 'primevue/usetoast';
    import { errorMessage } from '@/constants/ErrorMessages.js'; 
    import Loading from 'vue-loading-overlay';
    import Image from 'primevue/image';
    import { buttonsContent } from '@/constants/LabelContent.js';
    import { confirmationMessage } from '@/constants/ConfirmationMessages';

    const toast = useToast();
    const { dispatch, getters } = useStore(); 
    
    const ALL = {
        title: 'Mostrar todos',
        id: 0
    }
    
    const isLoading= ref(false);
    const agreements = ref([]);
    const tabs = ref([ALL]);
    const personId = computed(() => getters['auth/getLoggedInUser']);
    const userInfo = computed(() => getters['users/getUsers']);
    const categories = computed(() => getters['categories/getCategory']);
    const activeTab = ref(ALL.id);

    watch(activeTab, async (value) => {
        await dispatch('agreements/setActiveCategory', tabs.value[value]);
        agreements.value = getters['agreements/filteredAgreements'];}, 
        {
        immediate: true
    })
    
    const loadingOverlay = async () => {
        isLoading.value = true;
        try {
            await Promise.all([
                dispatch('categories/getActiveCategoriesWithAgreements'),          
                dispatch('agreements/getActiveAgreements'),
            ]);
            if (personId.value !== null) {
                await Promise.all([
                dispatch('users/getById')
            ]);
            }
            agreements.value = getters['agreements/filteredAgreements'];
        } catch (error) {
            console.error(error);
        } finally {
            isLoading.value = false;
        }
    }

    const requestMoreInfo = async (title) => {
        try {
            const userRequestedInformation = {
            PersonId: personId.value, 
            EmailAddress: userInfo.value.EmailAddress, 
            FullName:  userInfo.value.FullName, 
            Title: title, 
            
        };

        await dispatch('emails/requestAgreementInformationEmail',  {
            userRequestedInformation: userRequestedInformation
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

    onMounted(async () => {     
        await loadingOverlay(); 
        categories.value.forEach((category) => {
                tabs.value.push({
                    title: category.Description,
                    id: category.CategoryAgreementId
                })
            });
    })

 
</script>

<template>
    <toast-component />
    <loading v-model:active="isLoading" :lock-scroll="true" />
    <div class="card-bodies" v-if="agreements.length > 0">
        <TabView v-model:active-index="activeTab"> 
            <TabPanel v-for="tab in tabs" :key="tab.id" :header="tab.title">
                <div class="cards">
                    <Card class="card" v-for="agreement in agreements" :key="agreement.AgreementId" :agreement="agreement" >
                    <template #title > 
                        <Image class="card-image" :src="agreement.Image" width="250"  :alt="agreement.Title" preview/> 
                        <h2 class="card-title">
                            {{ agreement.Title}}                            
                        </h2>
                    </template>
                    <template #content>
                        <p class="card-content">
                        {{ agreement.Description}}
                        </p>
                    </template>
                    <template #footer v-if="personId">
                       <base-button :label="buttonsContent.IAMINTERESTED" @click="requestMoreInfo(agreement.Title)" :type="'button'" class="button" />
                    </template>
                    </Card>
                </div>
            </TabPanel>
        </TabView>
    </div>
</template>

<style scoped>
    .card-bodies {
        display: flex;
        flex-direction: column;
        padding: 3rem;
        width: 100%;
    }
    .p-tabview-nav-container{
        width: 100%;
    }
    .cards {
        display: flex;
        flex-direction: column;
        padding: 5rem;
        width: 100%;

    }
    
    .card {
        display: flex;
        width: 23rem;
        flex-direction: column;
        margin-bottom: 2rem;
        margin-right: 2rem;

    }
    
    .card-image {
        width: 100%!important;
        height: 100%!important;
        margin-bottom: 3rem!important;
        object-fit: cover!important;
    }
    
    .card-title {
        font-size: 1rem;
        font-weight: bold;
        margin: 0.5rem 0;
        text-transform: uppercase;
    }

    .button {
        display: flex;
        overflow: hidden;
        width: 10rem;
        margin-left: 3rem;
        color: white;
        text-align: center;
        flex-direction: row;
        align-items: center;
        justify-content: center;
    }

    @media (min-width: 768px) {
    
    .card-bodies {
        margin-right: 20rem;
        width: 1200px;
        flex-direction: row;
    }
    .cards {
        flex-direction: row;
        flex-wrap: wrap;
        justify-content: flex-start;
        overflow: none;
        padding: 0rem;
        margin-left: 3rem!important; 
             
    }
 
    
    .card {
        width: 25rem;
        flex-direction: column;
        margin-bottom: 2rem;
        margin-right: 2rem;
    }
    
    .card-title {
        font-size: 1rem;
        font-weight: bold;
        margin: 0.5rem 0;
        text-transform: uppercase;
    }

    .p-card-footer {
        flex-direction: column;

        align-items: center;
        justify-content: flex-end;
    }
}
</style>