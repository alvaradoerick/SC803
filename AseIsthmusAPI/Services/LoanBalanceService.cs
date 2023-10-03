using AseIsthmusAPI.Data.AseIsthmusModels;
using AseIsthmusAPI.Data;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Microsoft.Data.SqlClient;

namespace AseIsthmusAPI.Services
{
    public class LoanBalanceService
    {
        private readonly AseItshmusContext _context;
        public LoanBalanceService(AseItshmusContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<sp_GetLoanBalance_Result>?> GetBalanceByUser(string employeeCode)
        {
            var personIdParameter = new SqlParameter("@personId", employeeCode);
            var results = await _context.Sp_GetLoanBalances
                  .FromSqlRaw("EXEC sp_GetLoanBalance @personId", personIdParameter)
                  .ToListAsync();

            return results.AsEnumerable();
        }

        public async Task<LoanBalance?> Create(LoanBalance balance)
        {
            if (balance == null) return null;
            _context.LoanBalances.Add(balance);
            await _context.SaveChangesAsync();

            return balance;
        }

    }
}
