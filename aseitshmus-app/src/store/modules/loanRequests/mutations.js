export default {
    setErrorResponse(state, errorResponse) {
        state.errorResponse = errorResponse;
    },

    clearErrorResponse(state) {
        state.errorResponse = null;
    },

    setLoan(state, loans) {
        state.loans = loans;
    },
    setLoanCalculation(state, loanCalculation) {
        state.loanCalculation = loanCalculation;
    },
}