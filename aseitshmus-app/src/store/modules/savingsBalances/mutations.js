export default {
    setBalance(state, balance) {
        state.balance = balance;
    },

    setErrorResponse(state, errorResponse) {
        state.errorResponse = errorResponse;
    },

    clearErrorResponse(state) {
        state.errorResponse = null;
    },

}