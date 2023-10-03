using AseIsthmusAPI.Data;
using AseIsthmusAPI.Data.AseIsthmusModels;
using AseIsthmusAPI.Data.DTOs;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace AseIsthmusAPI.Services
{
    public class ContributionBalanceService
    {
        private readonly AseItshmusContext _context;

        public ContributionBalanceService(AseItshmusContext context)
        {
            _context = context;
        }

        public async Task<sp_GetContributionBalance_Result> GetByUser(string employeeCode)
        {
            var personIdParameter = new SqlParameter("@personId", SqlDbType.NVarChar, 12)
            {
                Value = employeeCode
            };

            var contributionBalanceIdParameter = new SqlParameter("@contributionBalanceId", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output,
            };

            var numberIdParameter = new SqlParameter("@numberId", SqlDbType.NVarChar, 12)
            {
                Direction = ParameterDirection.Output,
            };

            var nameParameter = new SqlParameter("@name", SqlDbType.NVarChar, 50)
            {
                Direction = ParameterDirection.Output,
            };

            var deductedDateParameter = new SqlParameter("@deductedDate", SqlDbType.Date)
            {
                Direction = ParameterDirection.Output,
            };

            var approvedDateParameter = new SqlParameter("@approvedDate", SqlDbType.Date)
            {
                Direction = ParameterDirection.Output,
                Value = employeeCode
            };

            var employeeContributionParameter = new SqlParameter("@employeeContribution", SqlDbType.Decimal)
            {
                Direction = ParameterDirection.Output,
                Precision = 18,
                Scale = 2
            };
            var employerContributionParameter = new SqlParameter("@employerContribution", SqlDbType.Decimal)
            {
                Direction = ParameterDirection.Output,
                Precision = 18,
                Scale = 2
            };
            var totalEmployeeContributionParameter = new SqlParameter("@totalEmployeeContribution", SqlDbType.Decimal)
            {
                Direction = ParameterDirection.Output,
                Precision = 18,
                Scale = 2
            };
            var totalEmployerContributionParameter = new SqlParameter("@totalEmployerContribution", SqlDbType.Decimal)
            {
                Direction = ParameterDirection.Output,
                Precision = 18,
                Scale = 2
            };
            var totalContributionParameter = new SqlParameter("@totalContribution", SqlDbType.Decimal)
            {
                Direction = ParameterDirection.Output,
                Precision = 18,
                Scale = 2
            };

            await _context.Database.ExecuteSqlRawAsync(
                "EXEC sp_GetContributionBalance @personId, @contributionBalanceId OUTPUT, @numberId OUTPUT, @name OUTPUT, @deductedDate OUTPUT, @approvedDate OUTPUT, @employeeContribution OUTPUT" +
                ", @employerContribution OUTPUT, @totalContribution OUTPUT, @totalEmployeeContribution OUTPUT, @totalEmployerContribution OUTPUT",
                personIdParameter,
                contributionBalanceIdParameter,
                numberIdParameter,
                nameParameter,
                deductedDateParameter,
                approvedDateParameter,
                employeeContributionParameter,
                employerContributionParameter,
                totalContributionParameter,
                totalEmployeeContributionParameter,
                totalEmployerContributionParameter
            );

            var contributionBalanceResult = new sp_GetContributionBalance_Result
            {
                ContributionBalanceId = contributionBalanceIdParameter.Value.Equals(DBNull.Value) ? 0 : (int)contributionBalanceIdParameter.Value,
                NumberId = numberIdParameter.Value.Equals(DBNull.Value) ? string.Empty : (string)numberIdParameter.Value,
                Name = nameParameter.Value.Equals(DBNull.Value) ? string.Empty : (string)nameParameter.Value,
                DeductedDate = deductedDateParameter.Value.Equals(DBNull.Value) ? default(DateTime) : (DateTime)deductedDateParameter.Value,
                ApprovedDate = approvedDateParameter.Value.Equals(DBNull.Value) ? default(DateTime) : (DateTime)approvedDateParameter.Value,
                EmployeeContribution = employeeContributionParameter.Value.Equals(DBNull.Value) ? 0m : (decimal)employeeContributionParameter.Value,
                EmployerContribution = employerContributionParameter.Value.Equals(DBNull.Value) ? 0m : (decimal)employerContributionParameter.Value,
                TotalEmployeeContribution = totalEmployeeContributionParameter.Value.Equals(DBNull.Value) ? 0m : (decimal)totalEmployeeContributionParameter.Value,
                TotalEmployerContribution = totalEmployerContributionParameter.Value.Equals(DBNull.Value) ? 0m : (decimal)totalEmployerContributionParameter.Value,
                TotalContribution = totalContributionParameter.Value.Equals(DBNull.Value) ? 0m : (decimal)totalContributionParameter.Value,
              
            };

            return contributionBalanceResult;

        }

        public async Task<ContributionBalance> Create(ContributionBalance balance)
        {
                _context.ContributionBalances.Add(balance);
                await _context.SaveChangesAsync();

                return balance;
           
        }
    }
}
