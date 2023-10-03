import { api } from '@/api/AxiosInterceptors.js';

export default {

    async getContributionByEmployee({
        commit, rootGetters
    }) {
        const personId = rootGetters['auth/getLoggedInUser'];
        const response = await api.get(`/ContributionBalance/${personId}`);   
        const balanceData = response.data;
        commit('setBalance', balanceData);
        return balanceData;
    },



};