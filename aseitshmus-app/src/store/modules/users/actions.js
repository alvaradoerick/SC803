import {api} from '@/api/AxiosInterceptors.js';

export default {

    async getAll({ commit }) {
        try {
            const response = await api.get('/user');
            const users = response.data;
            commit("setUsers", users);
            return response;
        } catch (error) {
            const errorMessage = error.response.data.error;
            commit('setErrorResponse', errorMessage);
        }
    },

    //gets data from the selected row. Admin purposes
    async getUserById({
        commit
    }, payload) {
        const userId = payload.rowId;
        const response = await api.get(`/user/${userId}`);   
        const userData = response.data;
        commit('setUsers', userData);
        return userData;
    },

    // gets the data for logged in user
    async getById({
        commit,
        rootGetters
    }) {
        const userId = rootGetters['auth/getLoggedInUser'];
        const response = await api.get(`/user/${userId}`);   
        const userData = response.data;
        commit('setUsers', userData);
        return userData;
    },

    async deleteUser({
        commit
    }, payload) {
        try {
            const userId = payload.rowId;
            const response = await api.delete(`/user/${userId}`);  
            return response;
        } catch (error) {
            const errorMessage = error.response.data.error;
            commit('setErrorResponse', errorMessage);
        }
    },

    //Method for user to update their own profile
    async patchProfile({
        rootGetters, 
        commit
    }, payload) {
        try {
            const personalInfo = payload.personalInfo;
            const userId = rootGetters['auth/getLoggedInUser'];
            const response = await api.patch(`/user/${userId}`,
            personalInfo);  
            return response
        } catch (error) {
            const errorMessage = error.response.data.error;
            commit('setErrorResponse', errorMessage);
        }
    },

    // Method for admin to update user
    async patchUser({
        commit
    }, payload) {
        try {
            const personId = payload.PersonId;
            const userInfo = payload.userInfo
            const response = await api.patch(`/user/employee/${personId}`,userInfo);   
            return response
        } catch (error) {
            const errorMessage = error.response.data.error;
            console.log(errorMessage)
            commit('setErrorResponse', errorMessage);
        }
    },

    // Activate or deactivate accounts
    async patchUserStatus({
        commit
    }, payload) {
        try {
            const personId = payload.PersonId;
            const response = await api.patch(`/user/activateuser/${personId}`);   
            return response
        } catch (error) {
            const errorMessage = error.response.data.error;
            commit('setErrorResponse', errorMessage);
        }
    },

    // User Resets password authenticated
    async resetPasswordAuthenticated({
        commit,
        rootGetters
    }, payload) {
        try {
            const userId = rootGetters['auth/getLoggedInUser'];
            const resetData = payload.resetData;
            const response = await api.patch(`/password/resetPassword/${userId}`,resetData);   
            return response;
        } catch (error) {
            const errorMessage = error.response.data.error;
            commit('setErrorResponse', errorMessage);
        }
    },
}