using AseIsthmusAPI.Data;
using AseIsthmusAPI.Data.AseIsthmusModels;
using AseIsthmusAPI.Data.DTOs;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto.Agreement;

namespace AseIsthmusAPI.Services
{
    public class SavingsRequestService
    {
        private readonly AseItshmusContext _context;

        #region Conversion methods
        private SavingsRequestDataDto ConvertToDto(SavingsRequest modelData)
        {

            return new SavingsRequestDataDto
            {
                SavingsRequestId = modelData.SavingsRequestId,
                PersonId = modelData.PersonId,
                Name = $"{modelData.Person.FirstName} {modelData.Person.LastName1} {modelData.Person.LastName2}",
                NumberId = modelData.Person.NumberId,
                SavingsTypeId = modelData.SavingsTypeId,
                SavingsTypeName = modelData.SavingsType.Description,
                ApplicationDate = modelData.ApplicationDate,
                Amount = modelData.Amount,
                IsActive = modelData.IsActive,
                Comments = modelData.Comments,
                IsCanceled = modelData.IsCanceled,
                ApprovedDate = modelData.ApprovedDate,
                IsApproved = modelData.IsApproved
            };
        }

        #endregion
        public SavingsRequestService(AseItshmusContext context)
        {
            _context = context;
        }


        public async Task<IEnumerable<SavingsRequestDataDto>> GetAll()
        {

            List<SavingsRequest> requests = await _context.SavingsRequests
                .Include(u => u.Person)
                .Include(t => t.SavingsType)
                .OrderByDescending(a => a.ApplicationDate)
                .ToListAsync();
            var dtoRequest = new List<SavingsRequestDataDto>();
            if (dtoRequest is null)
            {
                return null;
            }
            foreach (var request in requests)
            {
                var dto = ConvertToDto(request);
                dtoRequest.Add(dto);
            }

            return dtoRequest;

        }

        public async Task<SavingsRequestDataDto?> GetById(int id)
        {
            return await _context.SavingsRequests.Where(a => a.SavingsRequestId == id).
                Select(a => new SavingsRequestDataDto
                {
                    SavingsRequestId = a.SavingsRequestId,
                    PersonId = a.PersonId,
                    Name = $"{a.Person.FirstName} {a.Person.LastName1} {a.Person.LastName2}",
                    NumberId = a.Person.NumberId,
                    SavingsTypeId = a.SavingsTypeId,
                    SavingsTypeName = a.SavingsType.Description,
                    ApplicationDate = a.ApplicationDate,
                    Amount = a.Amount,
                    IsActive = a.IsActive,
                    ApprovedDate = a.ApprovedDate,
                    IsApproved = a.IsApproved,
                    IsCanceled = a.IsCanceled,
                    Comments = a.Comments
                }).SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<SavingsRequestDataDto>?> GetByEmployee(string employeeCode)
        {
            List<SavingsRequest> requests = await _context.SavingsRequests
           .Include(u => u.Person)
           .Include(t => t.SavingsType)
           .Where(a => a.PersonId == employeeCode)
           .OrderByDescending(a => a.ApplicationDate)
           .ToListAsync();
            var dtoRequest = new List<SavingsRequestDataDto>();
            if (dtoRequest is null)
            {
                return null;
            }
            foreach (var request in requests)
            {
                var dto = ConvertToDto(request);
                dtoRequest.Add(dto);
            }

            return dtoRequest;
        }

        public async Task<SavingsRequest> UpdateSaving(int id, ManageSavingsRequestByAdminDto saving) {
            var existingSaving = await _context.SavingsRequests.FindAsync(id);

            if (existingSaving is not null)
            {
              
                existingSaving.IsApproved = saving.IsApproved;
                existingSaving.ApprovedDate = DateTime.Now;
                existingSaving.Amount = saving.Amount;
                existingSaving.Comments = saving.Comments;
                if (saving.IsApproved == false) {
                    existingSaving.IsActive = false;
                }
                else {
                    existingSaving.IsActive = true;
                }
               
               
               

                await _context.SaveChangesAsync();
                return existingSaving;
            }
            else return null;
        }

        public async Task<SavingsRequest> CancelSavings(int id, CancelSavingsRequestDto cancelSavings)
        {
            var existingSaving = await _context.SavingsRequests.FindAsync(id);

            if (existingSaving is not null)
            {
                existingSaving.IsActive = false;
                existingSaving.IsCanceled = true;
                existingSaving.Comments = cancelSavings.Comments;

                await _context.SaveChangesAsync();
                return existingSaving;
            }
            else return null;
        }

        public async Task<SavingsRequest> Create(string id, ManageSavingsRequestDto savingsRequest)
        {
            var savings = new SavingsRequest
            {
                PersonId = id,
                SavingsTypeId = savingsRequest.SavingsTypeId,
                Amount = savingsRequest.Amount,
                ApplicationDate = DateTime.Now,
                IsActive = false,
            };

            _context.SavingsRequests.Add(savings);
            await _context.SaveChangesAsync();

            return savings;
        }

        public async Task Delete(int id)
        {
            var savingsToDelete = await _context.SavingsRequests.Where(a => a.SavingsRequestId == id).FirstOrDefaultAsync();

            if (savingsToDelete is not null)
            {
                _context.SavingsRequests.Remove(savingsToDelete);
                await _context.SaveChangesAsync();
            }
        }
    }
}
