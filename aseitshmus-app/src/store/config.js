import auth from "./modules/auth";
import users from "./modules/users";
import agreements from "./modules/agreements";
import beneficiaries from "./modules/beneficiaries";
import categories from "./modules/categories";
import loanTypes from "./modules/loanTypes";
import savingsTypes from "./modules/savingsTypes";
import savingsRequests from "./modules/savingsRequests";
import loanRequests from "./modules/loanRequests";
import contributionBalances from "./modules/contributionBalances";
import loansBalances from "./modules/loansBalances";
import savingsBalances from "./modules/savingsBalances";
import bulkImports from "./modules/bulkImports";
import emails from "./modules/emails";

export default {
    namespaced: true,
    modules:
    {
        auth,
        users,
        agreements,
        beneficiaries,
        categories,
        loanTypes,
        savingsTypes,
        savingsRequests,
        loanRequests,
        contributionBalances,
        loansBalances,
        savingsBalances,
        bulkImports,
        emails
    },

}
