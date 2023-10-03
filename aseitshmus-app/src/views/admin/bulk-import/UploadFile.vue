<script setup>
    import { useRouter } from 'vue-router';
    import { ref, computed } from 'vue';
    import { useToast } from 'primevue/usetoast';
    import { useStore } from 'vuex'
    import { errorMessage } from '@/constants/ErrorMessages.js';

    const router = useRouter();
    const toast = useToast();
    const store = useStore();

    const sendLabel = 'Enviar';
    const fileInput = ref(null);
    const selectedFile = ref(null); 

    const isDragging = ref(false);

    const handleFileUpload = () => {
        const fileInputRef = fileInput.value;
        selectedFile.value = fileInputRef.files[0];
    };

    const importResponse = computed(() => {
        return store.getters["bulkImports/getErrorResponse"];
    });

    const selectedFileName = computed(() => {
        return selectedFile.value ? selectedFile.value.name : '';
    });

const clearSelectedFile = () => {
    selectedFile.value = null;
    const fileInputRef = fileInput.value;
    fileInputRef.value = '';
  };
    
const handleDragOver = (event) => {
  event.preventDefault();
  isDragging.value = true;
};

const handleDragLeave = (event) => {
  event.preventDefault();
  isDragging.value = false;
};

const handleDrop = (event) => {
  event.preventDefault();
  isDragging.value = false;
  selectedFile.value = event.dataTransfer.files[0];
  console.log( selectedFile.value)
};

const openFileDialog = () => {
  const fileInputRef = fileInput.value;
  selectedFile.value = fileInputRef.files[0];
  console.log( selectedFile.value)
  fileInputRef.click();
};

const storeImport = async () => {
        const file = selectedFile.value;
        if (file) {
            const formData = new FormData();
            formData.append('file', file);
            await store.dispatch('bulkImports/uploadImport', formData, {
                headers: {
                    'Content-Type': 'multipart/form-data',
                },
            });
        }
  };

    const uploadExcelFile = async () => {
        try {
            await storeImport();
            if (importResponse.value !== null) {
                toast.add({
                    severity: 'error',
                    detail: importResponse.value,
                    life: 3000
                });
                store.commit('auth/clearErrorResponse');
            } else {
                toast.add({
                    severity: 'success',
                    detail: 'El archivo ha sido subido correctamente.',
                    life: 3000,
                });
                await new Promise((resolve) => setTimeout(resolve, 1000));
                router.push({
                    name: 'dashboard'
                });
            }
        } catch (error) {
            toast.add({
                severity: 'error',
                detail: errorMessage.ERROR500,
                life: 3000,
            });
        }
    }

</script>

<template>
    <div class="main">
        <toast-component />
        <div class="card-body custom-card-body">
        <div class="dropzone-container" @dragover="handleDragOver" @dragleave="handleDragLeave" @drop="handleDrop">
            <input ref="fileInput" type="file" id="myfile" name="myfile" class="hidden-input" @change="handleFileUpload"
                accept=".xlsx" />

            <label for="fileInput" class="file-label">
                <div v-if="isDragging">Release to drop files here.</div>
                <div v-else>
                    <div v-if="selectedFileName">{{ selectedFileName }}
                        <button v-if="selectedFile" class="clear-button" @click="clearSelectedFile">borrar</button>
                    </div>
                    <div v-else>
                        Inserte el archivo deseado o <u @click="openFileDialog">haz clic aqui</u> para subir el archivo.
                    </div>
                </div>
            </label>
        </div>
        <div class="actions">
            <base-button :label="sendLabel" @click="uploadExcelFile" :small="true" :type="'submit'" />
        </div>
    </div>
    </div>
</template>

<style scoped>

::v-deep .card-body {
    min-width: 400px!important; 
}
.main {
    display: flex;
    justify-content: center;
    align-items: center;
    border: 1px solid #ebebeb;
    flex-direction: column;
    padding: 2rem;
    min-width: 400px!important; 
}

.dropzone-container {
    padding: 4rem;
    margin-bottom: 2rem;
    background: #f7fafc;
    border: 1px solid #e2e8f0;
    min-width: 400px!important; 
}

.actions {
    align-self: flex-end;
}

.actions button {
    flex: 1;
    margin-left: 4rem;
}

.hidden-input {
    opacity: 0;
    overflow: hidden;
    position: absolute;
    width: 1px;
    height: 1px;
}

.file-preview {
    display: flex;
    align-items: center;
}

.clear-button {
    cursor: pointer;
    border: none;
    align-items: center;
    background-color: transparent;
    color: blue;
    text-decoration: underline;
}

</style>