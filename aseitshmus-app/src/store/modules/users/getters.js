export default {
     getUsers(state) {
        return state.users;
    },
    getBeneficiaries(state) {
        return state.beneficiaries;
    },
    find(state) {
        return (id) => {
            return state.users.find(user => user.PersonId ==  id) || {};
        }
    },
    getErrorResponse(state) {
        return state.errorResponse;
    },


}