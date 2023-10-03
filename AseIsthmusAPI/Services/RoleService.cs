using AseIsthmusAPI.Data;
using AseIsthmusAPI.Data.AseIsthmusModels;
using AseIsthmusAPI.Data.DTOs;
using AseIsthmusAPI.Repositories;
using AseIsthmusAPI.Repositories.Interfaces;
using AseIsthmusAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AseIsthmusAPI.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        public RoleService(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<IEnumerable<Role>> GetAllAsync()
        {
            return await _roleRepository.GetAllAsync();
        }

        public async Task<string> GetRoleDescriptionByIdAsync(int id)
        {
            return await _roleRepository.GetRoleDescriptionByIdAsync(id);
        }
    }
}