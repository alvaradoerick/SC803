import { api, setAuthToken, resetAuthToken } from '@/api/AxiosInterceptors.js';

export default {

    async register({
        dispatch,
        commit
    }, payload) {
        try {
            const personalInfo = payload.personalInfo;
            const workInfo = payload.workInfo;
            const addressInfo = payload.addressInfo;
            const beneficiaryInfo = payload.beneficiaryInfo;
            const response = await api.post(`/user`, {
                    ...personalInfo,
                    ...workInfo,
                    ...addressInfo,
                }
            )
            const PersonId = response.data.PersonId
            await dispatch('beneficiaries/addBeneficiaries', {
                PersonId,
                beneficiaryInfo
            },{ root: true });

            return response;
        } catch (error) {
            const errorMessage = error.response.data.error;
            commit('setErrorResponse', errorMessage);
        }
    },

    async login({
        commit, dispatch
    }, payload) {
        try {            
            const response = await api.post(`/login/authenticate`, payload.formData);
            localStorage.setItem('token', response.data.Token);
            localStorage.setItem('loggedInUser', response.data.PersonId);
            localStorage.setItem('role', response.data.RoleId);
            
            setAuthToken();
            await  dispatch('fetchLoginData');
        } catch (error) {
            const errorMessage = error.response.data.error;
            commit('setErrorResponse', errorMessage);
        }
    },

   
    logout( { commit }) {
        resetAuthToken();
        localStorage.removeItem('token');
        localStorage.removeItem('loggedInUser');
        localStorage.removeItem('role');
        
        commit('clearData')
    },

    async fetchLoginData({ commit }) { 
            commit('setToken', localStorage.getItem('token'));
            commit('setLoggedInUser', localStorage.getItem('loggedInUser'));
            commit('setRole', localStorage.getItem('role'));
      },

    async resetPasswordUnauthenticated({
        commit,
    }, payload) {
        try {
            const resetData = payload.resetData;
            const response = await api.patch(`/password/resetPassword`, resetData )
            commit('setEmailToken', response.data.token);
            return response;
        } catch (error) {
            const errorMessage = error.response.data.error;
            commit('setErrorResponse', errorMessage);
        }
    },

};