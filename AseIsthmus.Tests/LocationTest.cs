using AseIsthmusAPI.Data.DTOs;
using AseIsthmusAPI.Repositories;
using AseIsthmusAPI.Repositories.Interfaces;

namespace AseIsthmus.Tests
{
    public class LocationTest : IClassFixture<TestSetupFixture>
    {
        private readonly AseItshmusContext dbContext;
        private readonly LocationRepository locationRepository;
        public LocationTest(TestSetupFixture fixture)
        {
            dbContext = fixture.DbContext;
            locationRepository = new LocationRepository(dbContext);
        }

        [Theory]
        [InlineData(7)]
        public async Task GetAllProvincesAsync_ReturnCountOfPovince(int provinceCount)
        {
            try
            {
                var res = await locationRepository.GetAllProvincesAsync();
                Assert.Equal(provinceCount, res.Count());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }

        [Theory]
        [InlineData(2)]

        public async Task GetAllCantonsByProvinceAsync_ReturnCountOfCantonsByProvinceId(int provinceId)
        {
            int countofCantons = 15;

            try
            {
                var res = await locationRepository.GetCantonsByProvinceAsync(provinceId);
                Assert.Equal(countofCantons, res.Count);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }

        [Theory]
        [InlineData(101)]
        public async Task GetAllDistrictsByCantonAsync_ReturnCountOfDistrictsByCantonId(int cantonId)
        {
            int countofDistricts = 11;
            try
            {
                var res = await locationRepository.GetDistrictsByCantonAsync(cantonId);
                Assert.Equal(countofDistricts, res.Count);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            } 
        }

        [Theory]
        [InlineData(10108)]
        public async Task GetDistrictInformationAsync_ReturnDistrictInformationByDistrictId(int districtId)
        {
            var districtInformation = new LocationDto { ProvinceId = 1, ProvinceName = "San José", CantonId = 101, CantonName = "San José", DistrictId = 10108, DistrictName = "Mata Redonda" };

            try {
                var res = await locationRepository.GetDistrictInformationAsync(districtId);

                Assert.Equal(districtInformation.ProvinceName, res.ProvinceName);

                Assert.Equal(districtInformation.CantonName, res.CantonName);

                Assert.Equal(districtInformation.DistrictName, res.DistrictName);

                Assert.Equal(districtInformation.ProvinceId, res.ProvinceId);

                Assert.Equal(districtInformation.CantonId, res.CantonId);

                Assert.Equal(districtInformation.DistrictId, res.DistrictId);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }


        }
    }
}
