using AseIsthmusAPI.Data.AseIsthmusModels;
using AseIsthmusAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace AseIsthmusAPI.Services
{
    public class ContributionUsageService
    {

        private readonly AseItshmusContext _context;

        public ContributionUsageService(AseItshmusContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<ContributionUsage>> GetAll()
        {
            return await _context.ContributionUsages.ToListAsync();
        }
    }
}
