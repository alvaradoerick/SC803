export default {
    setErrorResponse(state, errorResponse) {
        state.errorResponse = errorResponse;
    },

    clearErrorResponse(state) {
        state.errorResponse = null;
    },

    setCategory(state, categories) {
        state.categories = categories;
    },

}