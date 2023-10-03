export default {
    setErrorResponse(state, errorResponse) {
        state.errorResponse = errorResponse;
    },

    clearErrorResponse(state) {
        state.errorResponse = null;
    },

    setSavings(state, savings) {
        state.savings = savings;
    },
}