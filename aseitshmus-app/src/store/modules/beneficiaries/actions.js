import {api} from '@/api/AxiosInterceptors.js';

export default {

    async addBeneficiaries({
        commit
    }, payload) {
        try {
            const response = await api.post(`/users/${payload.PersonId}/beneficiary`,payload.beneficiaryInfo);
            return response;
        } catch (error) {
            const errorMessage = error.response.data.error;
            commit('setErrorResponse', errorMessage);
        }
    },
    async getBeneficiaries({
        commit
    },payload) {
       
        const PersonId = payload.PersonId;
        const response = await api.get(`/users/${PersonId}/beneficiary`);
        const beneficiariesData = response.data;
        commit('setBeneficiaries', beneficiariesData);
        return beneficiariesData;
    },

    async deleteBeneficiaries({
        commit
    }, payload) {
        try {
            const PersonId = payload.PersonId;
            const response = await api.delete(`/users/${PersonId}/beneficiary`);
            return response;
        } catch (error) {
            const errorMessage = error.response.data.error;
            commit('setErrorResponse', errorMessage);
        }
    },

    async deleteAndInsertBeneficiaries({ dispatch, commit }, payload) {
        try {
          await dispatch('deleteBeneficiaries', { PersonId: payload.PersonId });     
          const insertPayload = {
            PersonId: payload.PersonId,
            beneficiaryInfo: payload.beneficiaryInfo,
          };
          const response = await dispatch('addBeneficiaries', insertPayload);
          return response;
        } catch (error) {
          const errorMessage = error.response.data.error;
          commit('setErrorResponse', errorMessage);
        }
      },
      
}