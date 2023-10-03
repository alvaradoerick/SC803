using AseIsthmusAPI.Data.AseIsthmusModels;
using AseIsthmusAPI.Data;
using Microsoft.EntityFrameworkCore;
using AseIsthmusAPI.Data.DTOs;

namespace AseIsthmusAPI.Services
{
    public class LoansTypeService
    {
        private readonly AseItshmusContext _context;

        public LoansTypeService(AseItshmusContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<LoansType>> GetAll()
        {
            return await _context.LoansTypes.ToListAsync();
        }
        public async Task<LoanTypeDataDto?> GetById(int id)
        {
            return await _context.LoansTypes.Where(a => a.LoansTypeId == id).
               Select(a => new LoanTypeDataDto
               {
                   LoansTypeId = a.LoansTypeId,
                   LoanDescription = a.Description,
                   ContributionUsageId = a.ContributionUsageId,
                   PercentageEmployeeCont = a.PercentageEmployeeCont,
                   PercentageEmployerCont = a.PercentageEmployerCont,
                   Term = a.Term,
                   InterestRate = a.InterestRate,
                   IsActive = a.IsActive,
                   ContributionUsageDescription = a.ContributionUsage.Description
               }).SingleOrDefaultAsync();
        }
        public async Task<IEnumerable<LoansType>> GetAllActiveLoans()
        {
            var activeLoans = await _context.LoansTypes.Where(a => a.IsActive == true).ToListAsync();

            return activeLoans;
        }
        public async Task<LoansType> Create(ManageLoanTypeDto newLoanType)
        {
            var loanType = new LoansType
            {
               Description = newLoanType.LoanDescription,
            ContributionUsageId = newLoanType.ContributionUsageId,
            PercentageEmployeeCont = newLoanType.PercentageEmployeeCont,
            PercentageEmployerCont = newLoanType.PercentageEmployerCont,
            Term = newLoanType.Term,
            InterestRate = newLoanType.InterestRate,
            IsActive = newLoanType.IsActive,
        };
            _context.LoansTypes.Add(loanType);
            await _context.SaveChangesAsync();

            return loanType;
        }
        public async Task<LoansType?> Update(int id, ManageLoanTypeDto loanType)
        {

            var existingLoanType = await _context.LoansTypes.FindAsync(id);

            if (existingLoanType is not null)
            {
                existingLoanType.Description = loanType.LoanDescription;
                existingLoanType.ContributionUsageId = loanType.ContributionUsageId;
                existingLoanType.PercentageEmployeeCont = loanType.PercentageEmployeeCont;
                existingLoanType.PercentageEmployerCont = loanType.PercentageEmployerCont;
                existingLoanType.Term = loanType.Term;
                existingLoanType.InterestRate = loanType.InterestRate;
                existingLoanType.IsActive = loanType.IsActive;
                await _context.SaveChangesAsync();
                return existingLoanType;
            }
            else return null; 
           
        }
        public async Task Delete(int id)
        {
            var loanTypeToDelete = await _context.LoansTypes.Where(a => a.LoansTypeId == id).FirstOrDefaultAsync();

            if (loanTypeToDelete is not null)
            {
                _context.LoansTypes.Remove(loanTypeToDelete);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<bool> HasLoans(int id)
        {
            var loanCount = await _context.LoanRequests.Where(a => a.LoansTypeId == id).CountAsync();

            return loanCount > 0;
        }
    }
}