import { api } from '@/api/AxiosInterceptors.js';

export default {

    async uploadImport({
        commit
    }, formData) {
        try {
            const response = await api.post('excel',formData);
            return response;
        } catch (error) {
            const errorMessage = error.response.data.error;
            commit('setErrorResponse', errorMessage);
        }
    },
   
}