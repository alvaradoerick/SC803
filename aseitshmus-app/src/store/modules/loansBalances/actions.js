import { api } from '@/api/AxiosInterceptors.js';

export default {
    async getLoansBalanceByEmployee({
        commit, rootGetters
    }) {
        const personId = rootGetters['auth/getLoggedInUser'];
        const response = await api.get(`/LoanBalance/${personId}`);   
        const balanceData = response.data;
        commit('setBalance', balanceData);
        return balanceData;
    },

};