
using AseIsthmusAPI.Data.AseIsthmusModels;
using AseIsthmusAPI.Data.DTOs;

namespace AseIsthmusAPI.Repositories.Interfaces
{
    public interface ILocationRepository
    {
        Task<IEnumerable<Province>> GetAllProvincesAsync();
        public Task<List<LocationDto>> GetCantonsByProvinceAsync(int provinceId);
        public Task<List<LocationDto>> GetDistrictsByCantonAsync(int cantonId);
        public Task<LocationDto?> GetDistrictInformationAsync(int districtId);
    }
}
