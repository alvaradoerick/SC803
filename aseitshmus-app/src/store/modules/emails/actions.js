import { api  } from '@/api/AxiosInterceptors.js';

export default {
 
    async sendUserActivationEmail({
        commit
    }, payload) {
        try {
            const emailAddress = payload.email;
            const response = await api.post('/email/user-activation-email', emailAddress);
            return response;
        } catch (error) {
            const errorMessage = error.response.data.error;
            commit('setErrorResponse', errorMessage);
        }
    },

    async approveLoanEmail({
        commit
    }, payload) {
        try {
            const emailInformation = payload.emailInformation;
            const response = await api.post('/email/approved-loan-email', emailInformation);
            return response;
        } catch (error) {
            const errorMessage = error.response.data.error;
            commit('setErrorResponse', errorMessage);
        }
    },

    async rejectLoanEmail({
        commit
    }, payload) {
        try {
            const emailInformation = payload.emailInformation;
            const response = await api.post('/email/rejected-loan-email', emailInformation);
            return response;
        } catch (error) {
            const errorMessage = error.response.data.error;
            commit('setErrorResponse', errorMessage);
        }
    },

    async generatePasswordEmail({ commit }) {
        try {
            const response = await api.get('/email/forgot-password-email');
            return response;
        } catch (error) {
            const errorMessage = error.response.data.error;
            commit('setErrorResponse', errorMessage);
        }
    },


    async requestAgreementInformationEmail({
        commit
    }, payload) {
        try {
            const userRequestedInformation = payload.userRequestedInformation;
            const response = await api.post('/email/agreement-information-email', userRequestedInformation);
            return response;
        } catch (error) {
            const errorMessage = error.response.data.error;
            commit('setErrorResponse', errorMessage);
        }
    },

    async registerEmail({
        commit
    }, payload) {
        try {
            const emailInformation = payload.emailInformation;
            const response = await api.post('/email/register-user-email', emailInformation);
            return response;
        } catch (error) {
            const errorMessage = error.response.data.error;
            commit('setErrorResponse', errorMessage);
        }
    },

    async resetPasswordEmail({
        commit
    }, payload) {
        try {
            const emailAddress = payload.email;
            const response = await api.post('/email/reset-password-email', emailAddress);
            return response;
        } catch (error) {
            const errorMessage = error.response.data.error;
            commit('setErrorResponse', errorMessage);
        }
    },

    async requestLoanReviewEmail({
        commit
    }, payload) {
        try {
            const loanInformation = payload.loanInformation;
            const response = await api.post('/email/request-loan-review-email', loanInformation);
            return response;
        } catch (error) {
            const errorMessage = error.response.data.error;
            commit('setErrorResponse', errorMessage);
        }
    },

    async respondLoanReviewEmail({
        commit
    }, payload) {
        try {
            const loanInformation = payload.loanInformation;
            const response = await api.post('/email/respond-loan-review-email', loanInformation);
            return response;
        } catch (error) {
            const errorMessage = error.response.data.error;
            commit('setErrorResponse', errorMessage);
        }
    },

    async loanRequestedEmail({
        commit
    }, payload) {
        try {
            const userRequestedInformation = payload.userRequestedInformation;
            const response = await api.post('/email/loan-requested-email', userRequestedInformation);
            return response;
        } catch (error) {
            const errorMessage = error.response.data.error;
            commit('setErrorResponse', errorMessage);
        }
    },
};