<script setup>
    import useVuelidate from '@vuelidate/core'
    import { required } from '@vuelidate/validators'
    import { api } from '@/api/AxiosInterceptors.js';
    import { useStore } from 'vuex'
    import { useRouter } from 'vue-router';
    import { ref, onMounted } from 'vue';
    import { useToast } from 'primevue/usetoast';
    import Textarea from 'primevue/textarea';
    import { errorMessage } from '@/constants/ErrorMessages';
    import { confirmationMessage } from '@/constants/ConfirmationMessages.js';
    import { localeContent, buttonsContent } from '@/constants/LabelContent.js';
    import FileUpload from 'primevue/fileupload';
    import Button from 'primevue/button';

    const store = useStore();
    const router = useRouter();
    const toast = useToast();
 
    const loadingButton = ref(false);
    const selectedState = ref(1);
    const categories = ref([]);
    const selectedCategory = ref(null);
    const totalSize = ref(0);
    const totalSizePercent = ref(0);
    const files = ref([]);

    const status = ref([
        {
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

    const backToList = () => { router.push({ name: "agrementList" }); }

    const storeAgreement = async () => {
        const formData = new FormData();
        formData.append('Title', agreementData.value.Title);
        formData.append('Description', agreementData.value.Description);
        formData.append('CategoryAgreementId', agreementData.value.CategoryAgreementId);
        formData.append('IsActive', agreementData.value.IsActive === 2 ? false : true);
        formData.append('Image', files.value[0]);
        await store.dispatch('agreements/addAgreement', formData, {
            headers: {
                'Content-Type': 'multipart/form-data',
            },
        });
    }

    const fetchActiveCategories = async () => {
        try {
            const response = await api.get('categoryagreement/active-categories');
            categories.value = response.data;
            if (response.data.length > 0) {
                selectedCategory.value = response.data[0].CategoryAgreementId;
            }
        } catch (error) {
            console.error(error);
        }
    };

    const rules = {
        Title: {
            required
        },
        Description: {
            required
        },
        CategoryAgreementId: {
            required
        },
        IsActive: {
            required
        }
    }

    const v$ = useVuelidate(rules, agreementData);

    const validateForm = async () => {
        const result = await v$.value.$validate();
        if (!result) {
            if (v$.value.$errors[0].$validator === 'required') {
                toast.add({
                    severity: 'error',
                    detail: 'Todos los campos son requeridos.',
                    life: 3000
                });
            }
            return false
        }
        return true;
    }

    const onSend = async (event) => {
        event.preventDefault();
        const isValid = await validateForm();
        if (isValid) {
            try {      
                try {
                        loadingButton.value = true;
                        await Promise.all([
                         storeAgreement()              
                        ]);
                        toast.add({
                    severity: 'success',
                    detail: confirmationMessage.RECORDADDED,
                    life: 3000
                });
                await new Promise((resolve) => setTimeout(resolve, 1000));
                router.push({ name: 'agrementList' });
                    } catch (error) {
                        console.error(error);
                    } finally {
                        loadingButton.value = false;
                    }                       
            } catch (error) {
                toast.add({              
                    severity: 'error',
                    detail: errorMessage.ERROR500,
                    life: 3000
                });
            }
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

    const onSelectedFiles = (event) => {
        files.value = event.files;
        files.value.forEach((file) => {
            totalSize.value += parseInt(formatSize(file.size));
        }); 
        agreementData.value.Image =files.value[0]
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


    onMounted( async () => {
        await fetchActiveCategories();
    })

</script>

<template>
   <div class="main">
        <toast-component />
        <div class="form">
        <div class="header">
            <div class="form-row">
                <div class="p-float-label">
                <input-text placeholder="Convenio" class=" input-text form-margin-right" id="agreementName" type="text"
                    v-model="agreementData.Title" :class="{'p-invalid': v$?.Title?.$error}" />
                    <label for="agreementName">Convenio</label>
                </div>
                    <div class="p-float-label">
                <drop-down v-model="selectedState" :options="status" optionLabel="name" optionValue="value" :emptyFilterMessage="localeContent.NOAVAILABLEOPTION" 
                :emptyMessage="localeContent.NOAVAILABLEOPTION" placeholder="Estado" class="dropdown" id="status" :class="{'p-invalid': v$?.selectedState?.$error}" />
                    <label for="status">Estado</label>
                </div>
                    <div class="p-float-label form-margin-left">
                <drop-down v-model="selectedCategory" :options="categories" optionLabel="Description" :emptyFilterMessage="localeContent.NOAVAILABLEOPTION" :emptyMessage="localeContent.NOAVAILABLEOPTION"
                    optionValue="CategoryAgreementId" class="dropdownLarger " id="category"
                    :class="{'p-invalid': v$?.CategoryAgreementId?.$error}" />
                    <label for="category">Categoría</label>
                </div>
            </div>
            <div class="form-row">
                <div class="p-float-label">
                <Textarea id="description" placeholder="Descripción" v-model="agreementData.Description" rows="5"
                    cols="45" class="form-margin-right" :class="{'p-invalid': v$?.Description?.$error}" ></Textarea>
                    <label for="description">Descripción</label>
                </div>
            </div>             
              <div class="form-row" style="width: 40rem;">     
                <FileUpload style="width: 40rem;" name="demo[]"  @upload="onTemplatedUpload($event)" accept="image/*" :maxFileSize="1000000" @select="onSelectedFiles">
            <template #header="{ chooseCallback, clearCallback, files }" >
                <div class="flex flex-wrap justify-content-between align-items-center flex-1 gap-2" style="width: 40rem;">
                    <div class="flex gap-2">
                        <Button @click="chooseCallback()" icon="pi pi-image" class="text-white" rounded outlined></Button>
                        <Button @click="onClearTemplatingUpload(clearCallback)" icon="pi pi-times"  class="text-white"  rounded outlined  :disabled="!files || files.length === 0"></Button>
                    </div>
                </div>
            </template>
            <template #content="{ files,  removeFileCallback }">
                <div v-if="files.length > 0">

                        <div v-for="(file, index) of files" :key="file.name + file.type + file.size" class=" m-0 px-6 flex flex-column surface-border align-items-center gap-3">
                            <div>
                                <image-component role="presentation" :alt="file.name" :src="file.objectURL" class="shadow-2" preview width="350" /> 
                            </div>
                            <span class="font-semibold">{{ file.name }}</span>
                            <div>{{ formatSize(file.size) }}</div>
                            <Button icon="pi pi-times" @click="onRemoveTemplatingFile(file, removeFileCallback, index)" outlined rounded  class="text-white"  />
                        </div>

                </div>      
            </template>
            <template #empty>
                <div class="flex align-items-center justify-content-center flex-column">
                    <i class="pi pi-cloud-upload border-2 border-circle p-5 text-8xl text-400 border-400 " />
                    <p class="mt-4 mb-0"> Inserte el archivo deseado.</p>
                </div>
            </template>
        </FileUpload>
        </div>
    </div>
    <div class="actions">
        <base-button :label="buttonsContent.CANCEL" @click="backToList" :small="true" :type="'button'" />
        <base-button :label="buttonsContent.ADD" :loading="loadingButton" @click="onSend" :small="true" :type="'submit'" />
    </div> 
</div>
</div>
</template>

<style scoped>

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
}

Button{
     background-color: #077CBC !important;
        border-color:  #077CBC !important;
}
Button:hover,
    Button:focus{
        box-shadow: 0 0 0 2px white, 0 0 0 3px skyblue;
        color: white;
        background-color: #0b4666 !important;
    }
</style>