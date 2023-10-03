<script setup>
    import { useStore } from 'vuex'
    import { useRouter, useRoute } from 'vue-router';
    import { ref, onMounted, computed, onBeforeUnmount } from 'vue';
    import { useToast } from 'primevue/usetoast';
    import { required } from '@vuelidate/validators'
    import useVuelidate from '@vuelidate/core'
    import Textarea from 'primevue/textarea';
    import Loading from 'vue-loading-overlay';
    import { confirmationMessage } from '@/constants/ConfirmationMessages.js';
    import { errorMessage } from '@/constants/ErrorMessages.js';
    import { useConfirm } from "primevue/useconfirm";
    import { localeContent, buttonsContent } from '@/constants/LabelContent.js';
    import FileUpload from 'primevue/fileupload';
    import Button from 'primevue/button';

    const store = useStore();
    const router = useRouter();
    const route = useRoute();
    const toast = useToast();
    const confirm = useConfirm();
    const { dispatch, getters } = useStore();

    const isLoading= ref(false);
    const loadingButton = ref(false);
    const AgreementId = ref(route.params.id);
    const selectedState = ref(null);
    const selectedCategory = ref(null);
    const totalSize = ref(0);
    const totalSizePercent = ref(0);
    const files = ref([]);
    const hideExistingImage = ref(false);
    const existingImage = ref(null);
    
    const categoryName = computed(() => getters['categories/getCategory']); 
    
    const agreementList = () => { router.push({ name: "agrementList" }); }

    const status = ref([{
            name: 'Activo',
            value: 1
        },
        {
            name: 'Inactivo',
            value: 2
        }
    ]);

    const agreementData = ref({
        Title: null,
        Description: null,
        Image:  null,
        CategoryAgreementId: selectedCategory,
        IsActive: selectedState
    })

    const rules = {
        Title: { required },
        Description: { required },
        CategoryAgreementId: { required },
        IsActive: { required }
    }
 
    const getImageAsFile = async (imageUrl) => {
        const response = await fetch(imageUrl);
        const blob = await response.blob();
        const filename = imageUrl.substring(imageUrl.lastIndexOf('/') + 1);
        const file = new File([blob], filename, {
            type: blob.type
        });
        return file;
    };


    const storeAgreement = async () => {
        if (files.value.length > 0) {
            agreementData.value.Image = files.value[0];
        } else {
            const imageFile = await getImageAsFile(agreementData.value.Image);
            agreementData.value.Image = imageFile;
        }
        const formData = new FormData();
        formData.append('Title', agreementData.value.Title);
        formData.append('Description', agreementData.value.Description);
        formData.append('CategoryAgreementId', agreementData.value.CategoryAgreementId);
        formData.append('IsActive', agreementData.value.IsActive === 2 ? false : true);
        formData.append('Image', agreementData.value.Image);

        await store.dispatch('agreements/updateAgreement', {
            AgreementId: AgreementId.value,
            formData
        }, {
            headers: {
                'Content-Type': 'multipart/form-data',
            },
        });
    }

    const v$ = useVuelidate(rules, agreementData);


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

    const fetchAgreementData = async () => {
        await store.dispatch('agreements/getAgreementById', {
            rowId: AgreementId.value 
            }        
         );
        const agreements = store.getters["agreements/getAgreement"];
        try {
            agreementData.value.Title = agreements.Title;
            agreementData.value.Description = agreements.Description;
            selectedCategory.value = agreements.CategoryAgreementId;
            agreementData.value.Image = agreements.Image;
            agreementData.value.CategoryName = agreements.categoryName;
            selectedState.value = agreements.IsActive ? 1 : 2;
            existingImage.value = agreements.Image;
        } 
        catch (error) {
            console.error(error);
            toast.add({
                severity: 'error',
                detail: errorMessage.ERROR500,
                life: 3000
            });
        }  
    };

    const submitData = async () => {     
                try {
                    await storeAgreement();
                    toast.add({
                        severity: 'success',
                        detail: confirmationMessage.CHANGESSAVED,
                        life: 3000
                    });
                    await new Promise((resolve) => setTimeout(resolve, 1000));
                    router.push({ name: 'agrementList' });
                } catch (error) {
                    toast.add({
                        severity: 'error',
                        detail: errorMessage.ERROR500,
                        life: 3000
                    });
                }
    }

    const confirmUpdate = async (event) => {
        event.preventDefault();
        const isValid = await validateForm();
        if (isValid) {  
           
            confirm.require({
                header: confirmationMessage.CONFIRMATIONHEADER,
                message: confirmationMessage.CONFIRMUPDATE,
                acceptLabel: localeContent.YES,
                icon: 'pi pi-exclamation-triangle',
                accept: async () => {
                    try {
                        loadingButton.value = true;                    
                        await Promise.all([
                            submitData()
                        ]);
                    } catch (error) {
                        console.error(error);
                    } finally {
                        loadingButton.value = false;
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
        }
    };

    const loadingOverlay = async () => {
        isLoading.value = true;
        try {
            await Promise.all([
            fetchAgreementData(),
        await dispatch('categories/getActiveCategories')   
            ]);
        } catch (error) {
            console.error(error);
        } finally {
            isLoading.value = false;
        }
    }

    const onRemoveTemplatingFile = (file, removeFileCallback, index) => {
        removeFileCallback(index);
        totalSize.value -= parseInt(formatSize(file.size));
        totalSizePercent.value = totalSize.value / 10;
    };

    const onClearTemplatingUpload = (clear) => {
        clear();
        totalSize.value = 0;
        totalSizePercent.value = 0;
    };

    const clearExistingImage = () => {
        agreementData.value.Image = null;
        hideExistingImage.value = true
    };

    const onSelectedFiles = (event) => {
        files.value = event.files;
        files.value.forEach((file) => {
            totalSize.value += parseInt(formatSize(file.size));
        }); 
      
    };    

    const onTemplatedUpload = () => {
        toast.add({ severity: "info", summary: "Success", detail: "File Uploaded", life: 3000 });
    };

    const formatSize = (bytes) => {
        if (bytes === 0) return "0 B";
        const k = 1024;
        const sizes = ["B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB"];
        const i = Math.floor(Math.log(bytes) / Math.log(k));
        return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + " " + sizes[i];
    };

    onMounted(async () => {
        await loadingOverlay();       
    })

    onBeforeUnmount( () => {
  AgreementId.value = null; 
});

</script>

<template>
    <div class="main">
        <confirm-dialog></confirm-dialog>
        <loading v-model:active="isLoading" :lock-scroll="true" />
        <toast-component />
        <div class="form">
            <div class="header">
                <div class="form-row">
                    <div class="p-float-label">
                        <input-text placeholder="Convenio" class=" input-text form-margin-right" id="agreementName"
                            type="text" v-model="agreementData.Title" :class="{'p-invalid': v$?.Title?.$error}" />
                        <label for="agreementName">Convenio</label>
                    </div>
                    <div class="p-float-label">
                        <drop-down v-model="selectedState" :options="status" optionLabel="name" optionValue="value"
                            :emptyFilterMessage="localeContent.NOAVAILABLEOPTION"
                            :emptyMessage="localeContent.NOAVAILABLEOPTION" placeholder="Estado" class="dropdown"
                            id="status" :class="{'p-invalid': v$?.selectedState?.$error}" />
                        <label for="status">Estado</label>
                    </div>
                    <div class="p-float-label form-margin-left">
                        <drop-down v-model="selectedCategory" :options="categoryName" optionLabel="Description"
                            :emptyFilterMessage="localeContent.NOAVAILABLEOPTION"
                            :emptyMessage="localeContent.NOAVAILABLEOPTION" optionValue="CategoryAgreementId"
                            class="dropdownLarger" id="category"
                            :class="{'p-invalid': v$?.CategoryAgreementId?.$error}" />
                        <label for="category">Categoría</label>
                    </div>
                </div>
                <div class="form-row">
                    <div class="p-float-label">
                        <Textarea id="description" placeholder="Descripción" v-model="agreementData.Description"
                            rows="5" cols="45" class="form-margin-right"
                            :class="{'p-invalid': v$?.Description?.$error}"></Textarea>
                        <label for="description">Descripción</label>
                    </div>
                </div>

                <div v-if="!hideExistingImage">
                    <div>
                        <image-component :src="`${existingImage}?${Date.now()}`" :alt="agreementData.Title" preview  width="350"/>
                </div>
            </div>
                <div class="form-row" v-else>
                    <FileUpload style="width: 40rem;" name="demo[]" @upload="onTemplatedUpload($event)" accept="image/*"
                        :maxFileSize="1000000" @select="onSelectedFiles">
                        <template #header="{ chooseCallback, clearCallback, files }">
                            <div class="flex flex-wrap justify-content-between align-items-center flex-1 gap-2"
                                style="width: 40rem;">
                                <div class="flex gap-2">
                                    <Button @click="chooseCallback()" icon="pi pi-image" class="text-white" rounded
                                        outlined></Button>
                                    <Button @click="onClearTemplatingUpload(clearCallback)" icon="pi pi-times"
                                        class="text-white" rounded outlined
                                        :disabled="!files || files.length === 0"></Button>
                                </div>
                            </div>
                        </template>
                        <template #content="{ files,  removeFileCallback }">
                            <div v-if="files.length > 0">

                                <div v-for="(file, index) of files" :key="file.name + file.type + file.size"
                                    class=" m-0 px-6 flex flex-column surface-border align-items-center gap-3">
                                    <div>
                                        <image-component role="presentation" :alt="file.name" :src="file.objectURL"
                                            class="shadow-2" preview width="350"/>
                                    </div>
                                    <span class="font-semibold">{{ file.name }}</span>
                                    <div>{{ formatSize(file.size) }}</div>
                                    <Button icon="pi pi-times"
                                        @click="onRemoveTemplatingFile(file, removeFileCallback, index)" outlined
                                        rounded class="text-white" />
                                </div>
                            </div>
                        </template>
                        <template #empty>
                            <div class="flex align-items-center justify-content-center flex-column">
                                <i
                                    class="pi pi-cloud-upload border-2 border-circle p-5 text-8xl text-400 border-400 " />
                                <p class="mt-4 mb-0"> Inserte el archivo deseado.</p>
                            </div>
                        </template>
                    </FileUpload>
                </div>
            </div>
            <div class="actions">
                <base-button :label="buttonsContent.CLEARIMAGE" v-if="!hideExistingImage" color="red" small @click="clearExistingImage" :type="'button'" />
                <base-button :label="buttonsContent.CANCEL" small @click="agreementList" :type="'button'" />
                <base-button :label="buttonsContent.UPDATE" small :loading="loadingButton" @click="confirmUpdate"
                    :type="'submit'" />    
        </div>
    </div>
</div>
</template>

<style scoped="scoped">

.main {
    display: flex;
    justify-content: center;
    align-items: center;
    border: 1px solid #ebebeb;
    border-radius: 5px;
    padding: 2rem;
}

.form {
    display: flex;
    flex-direction: column;
    align-items: center;
    width: 100%;
}

.dropdownLarger {
    display: flex;
    width: 300px;
}


.form-row {
    margin-top: 2rem;
    display: flex;
    justify-content: space-between;
    align-self: center;
    margin-bottom: 2rem;
    width: 60%;
}

.form-margin-right {
    margin-right: 6rem;
}

.form-margin-left {
    margin-left: 6rem;
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
    width: 10.5rem !important;
}
</style>