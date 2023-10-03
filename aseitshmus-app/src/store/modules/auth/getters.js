export default {
    getToken(state) {
        return state.token;
    },

    isAuthenticated(state) {
        return !!state.token;
    },

    getLoggedInUser(state) {
        return state.loggedInUser;
    },
    getErrorResponse(state) {
        return state.errorResponse;
    },  
    getRole(state) {
        return state.role;
    },

    getEmailToken(state) {
        return state.emailToken;
    },

}