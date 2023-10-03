export default {
    setUsers(state, users) { 
        state.users = users;
    },
    setBeneficiaries(state, beneficiaries) { 
        state.beneficiaries = beneficiaries;
    },
    setErrorResponse(state, errorResponse) {
        state.errorResponse = errorResponse;
    },

    clearErrorResponse(state) {
        state.errorResponse = null;
    },
}