export default {
    setErrorResponse(state, errorResponse) {
        state.errorResponse = errorResponse;
    },

    clearErrorResponse(state) {
        state.errorResponse = null;
    },

    setBalance(state, balance) {
        state.balance = balance;
    },
}