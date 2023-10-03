export default {
    setToken(state, token) {
        state.token = token;
    },

    setLoggedInUser(state, loggedInUser) {
        state.loggedInUser = loggedInUser;
    },

    setRole(state, role) {
        state.role = role;
    },

    setErrorResponse(state, errorResponse) {
        state.errorResponse = errorResponse;
    },

    clearErrorResponse(state) {
        state.errorResponse = null;
    },

    clearEmailToken(state) {
        state.emailToken = null;
    },

    setEmailToken(state, emailToken) {
        state.emailToken = emailToken;
    },

    clearData(state) {
        state.token = null;
        state.role = null;
        state.loggedInUser = null;
    },
}