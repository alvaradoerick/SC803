import {api} from '@/api/AxiosInterceptors.js';

export default {

    async getAllCategories({
        commit
    }) {
        const response = await api.get(`/categoryagreement`); 
        const categoryData = response.data;
        commit('setCategory', categoryData);
        return categoryData;
    },

    async getActiveCategories({
        commit
    }) {
        const response = await api.get(`/categoryagreement/active-categories`); 
        const categoryData = response.data;
        commit('setCategory', categoryData);
        return categoryData;
    },

    async getActiveCategoriesWithAgreements({
        commit
    }) {
        const response = await api.get(`/categoryagreement/categories-with-agreements`); 
        const categoryData = response.data;
        commit('setCategory', categoryData);
        return categoryData;
    },
    
    async getCategoryById({
        commit
    },payload) {
        const categoryId = payload.rowId;
        const response = await api.get(`/categoryagreement/${categoryId}`); 
        const categoryData = response.data;
        commit('setCategory', categoryData);
        return categoryData;
    },

    //Post
    async addCategory({
        _
    }, payload) {
        console.log(_)
        const response = await api.post(`/categoryagreement`,payload.agreementCategory); 
            return response;
    },

    //Delete
    async deleteCategory({
        commit
    }, payload) {
        try {
            const categoryId = payload.rowId;
            const response = await api.delete(`/categoryagreement/${categoryId}`); 
            return response;
        } catch (error) {
            const errorMessage = error.response.data.error;
            commit('setErrorResponse', errorMessage);
        }
    },

    //Put or Patch
    async updateCategory(
        {
        commit
    }, payload) {
        try {
            const categoryId = payload.categoryId;
            const agreementCategory = payload.agreementCategory;
            const response = await api.put(`/categoryagreement/${categoryId}`,agreementCategory); 
            return response
        } catch (error) {
            const errorMessage = error.response.data.error;
            commit('setErrorResponse', errorMessage);
        }
    },

};