export default {
    getErrorResponse(state) {
        return state.errorResponse;
    },

    getLoans(state) {
        if (state.loans.length) {
            return state.loans;
        }
        return [];
    },

    getLoanCalculation(state) {
        return [state.loanCalculation];
    },

    find(state) {
        return (id) => {
            return state.loans.find(loan => loan.LoanRequestId === parseInt(id))
        }
    }

}