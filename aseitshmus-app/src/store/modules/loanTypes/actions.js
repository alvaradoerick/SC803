import {api} from '@/api/AxiosInterceptors.js';

export default {

    async getAllTypes({
        commit
    }) {
        const response = await api.get(`/LoansType`); 
        const typeData = response.data;
        commit('setType', typeData);
        return typeData;
    },

    async getActiveTypes({
        commit
    }) {
        const response = await api.get(`/LoansType/active-categories`); 
        const typeData = response.data;
        commit('setType', typeData);
        return typeData;
    },

    async getTypeById({
        commit
    }, payload) {
        const typeId = payload.rowId;
        const response = await api.get(`/LoansType/${typeId}`); 
        const typeData = response.data;
        commit('setType', typeData);
        return typeData;
    },

    //Post
    async addType({
        _
    }, payload) {
        console.log(_)
        const response = await api.post(`/LoansType`, payload.loanType); 
        return response;
    },

    //Delete
    async deleteType({
        commit
    }, payload) {
        try {
            const typeId = payload.rowId;
            const response = await api.delete(`/LoansType/${typeId}`); 
            return response;
        } catch (error) {
            const errorMessage = error.response.data.error;
            commit('setErrorResponse', errorMessage);
        }
    },

    //Put or Patch
    async updateType({
        commit
    }, payload) {
        try {
            const typeId = payload.typeId;
            const loanType = payload.loanType;
            const response = await api.put(`/LoansType/${typeId}`,loanType); 
            return response
        } catch (error) {
            const errorMessage = error.response.data.error;
            commit('setErrorResponse', errorMessage);
        }
    },
};