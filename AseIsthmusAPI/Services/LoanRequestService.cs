using AseIsthmusAPI.Data.AseIsthmusModels;
using AseIsthmusAPI.Data.DTOs;
using AseIsthmusAPI.Data;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Microsoft.Data.SqlClient;
using DocumentFormat.OpenXml.Wordprocessing;
using static AseIsthmusAPI.Data.DTOs.UpdateLoanRequestRespondReviewDto;

namespace AseIsthmusAPI.Services
{
    public class LoanRequestService
    {
        private readonly AseItshmusContext _context;
        public LoanRequestService(AseItshmusContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<LoanRequestDataDto>?> GetAll()
        {
            return await _context.LoanRequests
               .OrderByDescending(a => a.IsApproved == null)
                .ThenByDescending(a => a.IsActive)
                .Select(a => new LoanRequestDataDto
                {
                    LoanRequestId = a.LoanRequestId,
                    PersonId = a.PersonId,
                    Name = $"{a.Person.FirstName} {a.Person.LastName1} {a.Person.LastName2}",
                    NumberId = a.Person.NumberId,
                    LoansTypeId = a.LoansTypeId,
                    LoanTypeName = a.LoansType.Description,
                    RequestedDate = a.RequestedDate,
                    AmountRequested = a.AmountRequested,
                    Term = a.Term,
                    BankAccount = a.BankAccount,
                    IsActive = a.IsActive,
                    ApprovedDate = a.ApprovedDate,
                    IsApproved = a.IsApproved,
                    IsReviewRequired = a.IsReviewRequired,
                    IsReviewApproved = a.IsReviewApproved,
                    ReviewRequiredDate = a.ReviewRequiredDate,
                    Comments = a.Comments
                }).ToListAsync();
        }
        public async Task<LoanRequestDataDto?> GetById(int id)
        {
            return await _context.LoanRequests.Where(a => a.LoanRequestId == id).
                Select(a => new LoanRequestDataDto
                {
                    LoanRequestId = a.LoanRequestId,
                    PersonId = a.PersonId,
                    Name = $"{a.Person.FirstName} {a.Person.LastName1} {a.Person.LastName2}",
                    NumberId = a.Person.NumberId,
                    LoansTypeId = a.LoansTypeId,
                    LoanTypeName = a.LoansType.Description,
                    RequestedDate = a.RequestedDate,
                    AmountRequested = a.AmountRequested,
                    Term = a.Term,
                    BankAccount = a.BankAccount,
                    IsActive = a.IsActive,
                    ApprovedDate = a.ApprovedDate,
                    IsApproved = a.IsApproved,
                    IsReviewRequired = a.IsReviewRequired,
                    IsReviewApproved = a.IsReviewApproved,
                    ReviewRequiredDate = a.ReviewRequiredDate,
                    Comments = a.Comments
                })
                .OrderByDescending(a => a.RequestedDate)
                .SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<LoanRequestDataDto>?> GetByEmployee(string employeeCode)
        {
            return await _context.LoanRequests
              .OrderByDescending(a => a.IsApproved == null)
               .ThenByDescending(a => a.IsActive)
               .Select(a => new LoanRequestDataDto
               {
                   LoanRequestId = a.LoanRequestId,
                   PersonId = a.PersonId,
                   Name = $"{a.Person.FirstName} {a.Person.LastName1} {a.Person.LastName2}",
                   NumberId = a.Person.NumberId,
                   LoansTypeId = a.LoansTypeId,
                   LoanTypeName = a.LoansType.Description,
                   RequestedDate = a.RequestedDate,
                   AmountRequested = a.AmountRequested,
                   Term = a.Term,
                   BankAccount = a.BankAccount,
                   IsActive = a.IsActive,
                   ApprovedDate = a.ApprovedDate,
                   IsApproved = a.IsApproved,
                   IsReviewRequired = a.IsReviewRequired,
                   IsReviewApproved = a.IsReviewApproved,
                   ReviewRequiredDate = a.ReviewRequiredDate,
                   Comments = a.Comments
               })
               .Where(a => a.PersonId == employeeCode)
               .OrderByDescending(a => a.RequestedDate)
               .ToListAsync();


        }
       
        public async Task<bool> RequestLoanReview(int id, UpdateLoanRequestRequestReviewDto requestReviewComments)
        {
            var existingLoan = await _context.LoanRequests
                .Include(l => l.Person)
                .FirstOrDefaultAsync(l => l.LoanRequestId == id);

            if (existingLoan is not null)
            {           
                existingLoan.IsReviewRequired = true;
                existingLoan.Comments = requestReviewComments.Comments;
                existingLoan.IsReviewApproved = null;

                await _context.SaveChangesAsync();
                return true;

            }
            return false;

        }

        public async Task<bool> RespondLoanReview(int id, UpdateLoanRequestRespondReviewDto reviewRespond)
        {
            var existingLoan = await _context.LoanRequests
                .Include(l => l.Person)
                .FirstOrDefaultAsync(l => l.LoanRequestId == id);

            if (existingLoan is not null)
            {
                existingLoan.IsReviewRequired = null;
                existingLoan.Comments = reviewRespond.Comments;
                existingLoan.IsReviewApproved = reviewRespond.IsReviewApproved;
                existingLoan.ReviewRequiredDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return true;

            }
            return false;

        }

        public async Task<(string?, string?)> ApproveLoan(int id, UpdateLoanRequestByAdminDto loan)
        {
            var existingLoan = await _context.LoanRequests
                .Include(l => l.Person)
                .FirstOrDefaultAsync(l => l.LoanRequestId == id);

            if (existingLoan is not null)
            {
                var personId = existingLoan.PersonId;
                var associatedUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.PersonId == personId);

                if (associatedUser == null)
                {
                    return (null, null);
                }

                existingLoan.IsApproved = loan.IsApproved;
                existingLoan.IsReviewRequired = false;
                existingLoan.Comments = loan.Comments;

                if (loan.IsApproved == false)
                {
                    existingLoan.ApprovedDate = loan.ApprovedDate;
                    existingLoan.IsActive = false;
                }
                else
                {
                    existingLoan.ApprovedDate = loan.ApprovedDate;
                    existingLoan.IsActive = true;
                }

                var email = associatedUser.EmailAddress;
                var name = $"{associatedUser.FirstName} {associatedUser.LastName1} {associatedUser.LastName2}";

                await _context.SaveChangesAsync();

                return (email, name);
            }
            else
            {
                return (null, null);
            }
        }

        public async Task<LoanRequest> Create(string id, ManageLoanRequestDto loanRequest)
        {
            var savings = new LoanRequest
            {
                PersonId = id,
                LoansTypeId = loanRequest.LoansTypeId,
                AmountRequested = loanRequest.AmountRequested,
                Term = loanRequest.Term,
                BankAccount = loanRequest.BankAccount,
                RequestedDate = loanRequest.RequestedDate,
                IsActive = false,
            };

            _context.LoanRequests.Add(savings);
            await _context.SaveChangesAsync();

            return savings;
        }

        public async Task<sp_GetLoanCalculation_Result> GetLoanCalculation(LoanCalculationType loanCalculation)
        {
            var loanData = ConvertLoanCalculationTypeToDataTable(loanCalculation);

            var loanDataParameter = new SqlParameter("@loanData", SqlDbType.Structured)
            {
                TypeName = "dbo.LoanCalculationTypes",
                Value = loanData
            };
            var availEmployeeAmtParameter = new SqlParameter("@availEmployeeAmt", SqlDbType.Decimal)
            {
                Direction = ParameterDirection.Output,
                Precision = 18,
                Scale = 2
            };
            var availEmployerAmtParameter = new SqlParameter("@availEmployerAmt", SqlDbType.Decimal)
            {
                Direction = ParameterDirection.Output,
                Precision = 18,
                Scale = 2
            };
            var totalAvailAmountParameter = new SqlParameter("@totalAvailAmount", SqlDbType.Decimal)
            {
                Direction = ParameterDirection.Output,
                Precision = 18,
                Scale = 2
            };
            var biweeklyFeeParameter = new SqlParameter("@biweeklyFee", SqlDbType.Decimal)
            {
                Direction = ParameterDirection.Output,
                Precision = 18,
                Scale = 2
            };
            var totalAmtPayParameter = new SqlParameter("@totalAmtPay", SqlDbType.Decimal)
            {
                Direction = ParameterDirection.Output,
                Precision = 18,
                Scale = 2
            };

            var rateParameter = new SqlParameter("@rate", SqlDbType.Decimal)
            {
                Direction = ParameterDirection.Output,
                Precision = 18,
                Scale = 2
            };

            var termParameter = new SqlParameter("@currentTerm", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output,
            };


            await _context.Database.ExecuteSqlRawAsync(
                "EXEC sp_GetLoanCalculation @loanData, @availEmployeeAmt OUTPUT, @availEmployerAmt OUTPUT, @totalAvailAmount OUTPUT, @biweeklyFee OUTPUT, @totalAmtPay OUTPUT, @rate OUTPUT, @currentTerm OUTPUT",
                loanDataParameter,
                availEmployeeAmtParameter,
                availEmployerAmtParameter,
                totalAvailAmountParameter,
                biweeklyFeeParameter,
                totalAmtPayParameter,
                rateParameter,
                termParameter
            );

            var loanCalculationResult = new sp_GetLoanCalculation_Result
            {
                AvailEmployeeAmt = availEmployeeAmtParameter.Value.Equals(DBNull.Value) ? 0m : (decimal)availEmployeeAmtParameter.Value,
                AvailEmployerAmt = availEmployerAmtParameter.Value.Equals(DBNull.Value) ? 0m : (decimal)availEmployerAmtParameter.Value,
                TotalAvailAmount = totalAvailAmountParameter.Value.Equals(DBNull.Value) ? 0m : (decimal)totalAvailAmountParameter.Value,
                BiweeklyFee = biweeklyFeeParameter.Value.Equals(DBNull.Value) ? 0m : (decimal)biweeklyFeeParameter.Value,
                TotalAmtToPay = totalAmtPayParameter.Value.Equals(DBNull.Value) ? 0m : (decimal)totalAmtPayParameter.Value,
                Rate = rateParameter.Value.Equals(DBNull.Value) ? 0m : (decimal)rateParameter.Value,
                Term = termParameter.Value.Equals(DBNull.Value) ? 0 : (int.TryParse(termParameter.Value.ToString(), out int intValue) ? intValue : 0)
        };

            return loanCalculationResult;

        }

        public async Task Delete(int id)
        {
            var loanToDelete = await _context.LoanRequests.Where(a => a.LoanRequestId == id).FirstOrDefaultAsync();

            if (loanToDelete is not null)
            {
                _context.LoanRequests.Remove(loanToDelete);
                await _context.SaveChangesAsync();
            }
        }

        private DataTable ConvertLoanCalculationTypeToDataTable(LoanCalculationType loanCalculation)
        {
            DataTable dataTable = new DataTable();

            dataTable.Columns.Add("PersonId", typeof(string));
            dataTable.Columns.Add("LoansTypeId", typeof(int));
            dataTable.Columns.Add("Term", typeof(int));
            dataTable.Columns.Add("Amount", typeof(decimal));

            DataRow row = dataTable.NewRow();
            row["PersonId"] = loanCalculation.PersonId;
            row["LoansTypeId"] = loanCalculation.LoansTypeId;
            row["Term"] = loanCalculation.Term;
            row["Amount"] = loanCalculation.Amount;

            dataTable.Rows.Add(row);


            return dataTable;
        }
    }
}
