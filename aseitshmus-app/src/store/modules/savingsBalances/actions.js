import { api } from '@/api/AxiosInterceptors.js';

export default {
    async getSavingsBalanceByEmployee({
        commit, rootGetters
    }) {
        const personId = rootGetters['auth/getLoggedInUser'];
        const response = await api.get(`/SavingsBalance/${personId}`);   
        const balanceData = response.data;
        commit('setBalance', balanceData);
        return balanceData;
    },

};