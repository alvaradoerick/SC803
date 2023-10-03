using AseIsthmusAPI.Data;
using AseIsthmusAPI.Data.AseIsthmusModels;
using AseIsthmusAPI.Data.DTOs;
using AseIsthmusAPI.Repositories.Interfaces;

namespace AseIsthmusAPI.Repositories
{
    public class LocationRepository : ILocationRepository
    {
        private readonly AseItshmusContext _context;

        public LocationRepository(AseItshmusContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Province>> GetAllProvincesAsync()
        {

            return await _context.Provinces.ToListAsync();
        }

        public async Task<List<LocationDto>> GetCantonsByProvinceAsync(int provinceId)
        {
            return await _context.Cantons.Where(c => c.ProvinceId == provinceId).Select(a => new LocationDto
            {
                ProvinceId = a.ProvinceId,
                ProvinceName = a.Province.ProvinceName,
                CantonId = a.CantonId,
                CantonName = a.CantonName
            }).ToListAsync();
        }

        public async Task<List<LocationDto>> GetDistrictsByCantonAsync(int cantonId)
        {
            return await _context.Districts.Where(d => d.CantonId == cantonId)
                .Select(a => new LocationDto
                {
                    DistrictId = a.DistrictId,
                    DistrictName = a.DistrictName,
                    CantonId = a.CantonId,
                    CantonName = a.Canton.CantonName,
                    ProvinceId = a.Canton.ProvinceId,
                    ProvinceName = a.Canton.Province.ProvinceName
                }).ToListAsync();
        }

        public async Task<LocationDto?> GetDistrictInformationAsync(int districtId)
        {
            return await _context.Districts
       .Include(d => d.Canton)
       .ThenInclude(c => c.Province)
       .Where(d => d.DistrictId == districtId)
       .Select(a => new LocationDto
       {
           DistrictId = a.DistrictId,
           DistrictName = a.DistrictName,
           CantonId = a.CantonId,
           CantonName = a.Canton.CantonName,
           ProvinceId = a.Canton.ProvinceId,
           ProvinceName = a.Canton.Province.ProvinceName
       })
       .FirstOrDefaultAsync();
        }
    }
}