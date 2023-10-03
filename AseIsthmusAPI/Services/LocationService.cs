using AseIsthmusAPI.Data;
using AseIsthmusAPI.Data.AseIsthmusModels;
using AseIsthmusAPI.Data.DTOs;
using AseIsthmusAPI.Repositories.Interfaces;
using AseIsthmusAPI.Services.Interfaces;
using DocumentFormat.OpenXml.InkML;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;

namespace AseIsthmusAPI.Services
{
    public class LocationService : ILocationService
    {
        private readonly ILocationRepository _locationRepository;

        public LocationService(ILocationRepository locationRepository)
        {
            _locationRepository = locationRepository;
        }

        public async Task<IEnumerable<Province>> GetAllProvincesAsync()
        {
            return await _locationRepository.GetAllProvincesAsync();
        }

        public async Task<List<LocationDto>> GetCantonsByProvinceAsync(int provinceId)
        {
            return await _locationRepository.GetCantonsByProvinceAsync(provinceId);
        }

        public async Task<List<LocationDto>> GetDistrictsByCantonAsync(int cantonId)
        {
            return await _locationRepository.GetDistrictsByCantonAsync(cantonId);
        }

        public async Task<LocationDto?> GetDistrictInformationAsync(int districtId)
        {
            return await _locationRepository.GetDistrictInformationAsync(districtId);
        }
    }
}
