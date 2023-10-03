import {api} from '@/api/AxiosInterceptors.js';

export default {

    //Get
    async getAllLoans({
        commit
    }) {
        const response = await api.get('/loanrequest'); 
        const loanData = response.data;
        commit('setLoan', loanData);
        return loanData;
    },

    async getLoanById({
        commit
    }, payload) {
        const loanId = payload.rowId;
        const response = await api.get(`/loanrequest/${loanId}`); 
        const loanData = response.data;
        commit('setLoan', [loanData]);
        return loanData;
    },

    async getLoansByEmployee({
        commit,rootGetters
    }) {
        const personId =  rootGetters['auth/getLoggedInUser'];
        const response = await api.get(`/loanrequest/employee/${personId}`); 
        const loanData = response.data;
        commit('setLoan', [loanData]);
        return loanData;
    },

    //Post
    async getLoanCalculation({
        commit,rootGetters
    }, payload) {
        const PersonId = rootGetters['auth/getLoggedInUser'];
        const loanData = payload.loanData;
        const updatedLoanData = {
            ...loanData,
            PersonId: PersonId,
        };
        const response = await api.post(`/loanrequest/calculation`,updatedLoanData); 
        const loanDataResponse = response.data;
        commit('setLoanCalculation', loanDataResponse);
        return loanDataResponse;
    },

    async addLoanRequest({
        rootGetters
    }, payload) {
        const userId = rootGetters['auth/getLoggedInUser'];
        const response = await api.post(`/loanrequest/${userId}`,payload.loanRequest); 
        return response;
    },

    //Delete
    async deleteLoan({
        commit
    }, payload) {
        try {
            const loanRequestId = payload.rowId;
            const response = await api.delete(`/loanrequest/${loanRequestId}`); 
            return response;
        } catch (error) {
            const errorMessage = error.response.data.error;
            commit('setErrorResponse', errorMessage);
        }
    },

    //Put or Patch
    async updateLoan({
        _
    }, payload) {
        try {
            console.log(_);
            const loanRequestId = payload.loanRequestId;
            const loan = payload.loanState;
            const response = await api.patch(`/loanrequest/${loanRequestId}`,loan); 
            return response
        } catch (error) {
            console.log(error)
        }
    },

    async requestLoanReview({
        commit
    }, payload) {
        try {
            const loanRequestId = payload.rowId;
            const comment = payload.reviewComments;
            const response = await api.patch(`/loanrequest/request-review/${loanRequestId}`,comment);   
            return response
        } catch (error) {
            const errorMessage = error.response.data.error;
            commit('setErrorResponse', errorMessage);
        }
    },

    async respondLoanReview({
        commit
    }, payload) {
        try {
            const loanRequestId = payload.rowId;
            const comment = payload.reviewComments;
            const response = await api.patch(`/loanrequest/respond-review/${loanRequestId}`,comment);   
            return response
        } catch (error) {
            const errorMessage = error.response.data.error;
            commit('setErrorResponse', errorMessage);
        }
    },
};