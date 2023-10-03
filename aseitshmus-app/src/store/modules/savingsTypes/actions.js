import {api} from '@/api/AxiosInterceptors.js';

export default {

    async getAllTypes({
        commit
    }) {
        const response = await api.get(`/SavingsType`);   
        const typeData = response.data;
        commit('setType', typeData);
        return typeData;
    },

    async getActiveTypes({
        commit
    }) {
        const response = await api.get(`/SavingsType/active-categories`);  
        const typeData = response.data;
        commit('setType', typeData);
        return typeData;
    },

    async getTypeById({
        commit
    }, payload) {
        const typeId = payload.rowId;
        const response = await api.get(`/SavingsType/${typeId}`);  
        const typeData = response.data;
        commit('setType', typeData);
        return typeData;
    },

    //Post
    async addType({
_
    }, payload) {
        console.log(_)
        const response = await api.post(`/SavingsType`, payload.savingsType);  
        return response;
    },

    //Delete
    async deleteType({
        commit
    }, payload) {
        try {   
            const typeId = payload.rowId;
            const response = await api.delete(`/SavingsType/${typeId}`);
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
            const savingsType = payload.savingsType;
            const response = await api.put(`/SavingsType/${typeId}`, savingsType);  
            return response
        } catch (error) {
            const errorMessage = error.response.data.error;
            commit('setErrorResponse', errorMessage);
        }
    },
};