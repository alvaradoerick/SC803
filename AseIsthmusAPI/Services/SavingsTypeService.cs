using AseIsthmusAPI.Data;
using AseIsthmusAPI.Data.AseIsthmusModels;
using AseIsthmusAPI.Data.DTOs;
using Microsoft.EntityFrameworkCore;

namespace AseIsthmusAPI.Services
{
    public class SavingsTypeService
    {
        private readonly AseItshmusContext _context;

        public SavingsTypeService(AseItshmusContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SavingsType>> GetAll()
        {
            return await _context.SavingsTypes.ToListAsync();
        }
        public async Task<SavingsType?> GetById(int id)
        {
            return await _context.SavingsTypes.FindAsync(id);
        }
        public async Task<IEnumerable<SavingsType>> GetAllActiveSavings()
        {
            var currentDate = DateTime.Now.Date;
            var activeTypes = await _context.SavingsTypes
                .Where(a => a.IsActive == true && a.ApplicationDeadline >= currentDate)
                .ToListAsync();

            return activeTypes;
        }
        public async Task<SavingsType> Create(SavingsType newSavingsType)
        {
            _context.SavingsTypes.Add(newSavingsType);
            await _context.SaveChangesAsync();

            return newSavingsType;
        }
        public async Task Update(int id, SavingsType savingsType)
        {
            var existingSavingsType = await GetById(id);

            if (existingSavingsType is not null)
            {
                existingSavingsType.Description = savingsType.Description;
                existingSavingsType.ApplicationDeadline = savingsType.ApplicationDeadline;
                existingSavingsType.StartDate = savingsType.StartDate;
                existingSavingsType.EndDate = savingsType.EndDate;
                existingSavingsType.IsActive = savingsType.IsActive;
                await _context.SaveChangesAsync();
            }
        }
        public async Task Delete(int id)
        {
            var savingsTypeToDelete = await GetById(id);

            if (savingsTypeToDelete is not null)
            {
                _context.SavingsTypes.Remove(savingsTypeToDelete);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<bool> HasSavings(int id)
        {
            var savingsCount = await _context.SavingsRequests.Where(a => a.SavingsTypeId == id).CountAsync();

            return savingsCount > 0;
        }
    }
}
