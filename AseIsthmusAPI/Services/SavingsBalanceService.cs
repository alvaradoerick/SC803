using AseIsthmusAPI.Data.AseIsthmusModels;
using AseIsthmusAPI.Data;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Microsoft.Data.SqlClient;

namespace AseIsthmusAPI.Services
{
    public class SavingsBalanceService
    {
        private readonly AseItshmusContext _context;
        public SavingsBalanceService(AseItshmusContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<sp_GetSavingsBalance_Result>?> GetBalanceByUser(string employeeCode)
        {
            var personIdParameter = new SqlParameter("@personId", employeeCode);
            var results = await _context.Sp_GetSavingsBalances
                  .FromSqlRaw("EXEC sp_GetSavingsBalance @personId", personIdParameter)
                  .ToListAsync();

            return results.AsEnumerable();
        }

        public async Task<SavingsBalance> Create(SavingsBalance balance)
        {
            if (balance == null) throw new ArgumentNullException(nameof(balance));
            _context.SavingsBalances.Add(balance);
            await _context.SaveChangesAsync();

            return balance;
        }

    }
}
