using AseIsthmusAPI.Repositories;
using AseIsthmusAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Org.BouncyCastle.Crypto.Engines;

namespace AseIsthmus.Tests
{
    public class RoleTest : IClassFixture<TestSetupFixture>
    {
        private readonly AseItshmusContext dbContext;
        private readonly RoleRepository roleRepository;
        public RoleTest(TestSetupFixture fixture)
        {
            dbContext = fixture.DbContext;
            roleRepository = new RoleRepository(dbContext);
        }

        [Theory]
        [InlineData(4)]
        public async Task GetAllRolesAsync_ReturnRolesList(int count)
        {
            try
            {
                var result = await roleRepository.GetAllAsync();
                Assert.Equal(count, result.Count());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }

        [Theory]
        [InlineData(1)]
        public async void GetRoleDescriptionByIdAsync_ReturnRoleDescriptionByIdEntered(int roleId)
        {
            try
            {
                var result = await roleRepository.GetRoleDescriptionByIdAsync(roleId);
                Assert.Equal("Administrador", result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }

        [Theory]
        [InlineData(5)]
        public async void GetRoleDescriptionByIdAsync_ReturnErrorOfNotFound(int roleId)
        {
            try
            {
                var result = await roleRepository.GetRoleDescriptionByIdAsync(roleId);
                Assert.Null(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }   
    }
}