import {api} from '@/api/AxiosInterceptors.js';

export default {

    //Get
    async getAllAgreements({
        commit
    }) {
        const response = await api.get(`/agreement`);
        const agreementData = response.data;
        commit('setAgreement', agreementData);
        return agreementData;
    },

    async getActiveAgreements({
        commit
    }) {
        const response = await api.get(`/agreement/active-agreements`);  
        const agreementData = response.data;
        commit('setAgreement', agreementData);
        return agreementData;
    },
    

    async getAgreementById({
        commit
    },payload) {
        const agreementId = payload.rowId;
        const response = await api.get(`/agreement/${agreementId}`);
        const agreementData = response.data;
        commit('setAgreement', agreementData);
        return agreementData;
    },

    //Post
    async addAgreement({
        _
    }, formData) {
        console.log(_)
        const response = await api.post(`/agreement`,formData);
            return response;
    },
    
    //Set
   async setActiveCategory({ commit }, category) {
        commit('setActiveCategory', category);
    },

    //Delete
    async deleteAgreement({
        commit
    }, payload) {
        try {
            const agreementId = payload.rowId;
            const response = await api.delete(`/agreement/${agreementId}`);
            return response;
        } catch (error) {
            const errorMessage = error.response.data.error;
            commit('setErrorResponse', errorMessage);
        }
    },

     //Put or Patch
     async updateAgreement({
        _
    }, payload) {
        try {
            console.log(_)
            const agreementId = payload.AgreementId;
            const agreement = payload.formData;
            const response = await api.put(`/agreement/${agreementId}`,agreement);
            return response
        } catch (error) {
            console.log(error)
        }
    },

};