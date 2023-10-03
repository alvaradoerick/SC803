export default {
    setErrorResponse(state, errorResponse) {
        state.errorResponse = errorResponse;
    },

    clearErrorResponse(state) {
        state.errorResponse = null;
    },

    setAgreement(state, agreements) {
        state.agreements = agreements;
    },
    setActiveCategory(state, activeCategory ){
        state.activeCategory = activeCategory
    }
}