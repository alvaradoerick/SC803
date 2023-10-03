using AseIsthmusAPI.Data;
using AseIsthmusAPI.Data.AseIsthmusModels;
using AseIsthmusAPI.Repositories.Interfaces;

namespace AseIsthmusAPI.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly AseItshmusContext _context;

        public RoleRepository(AseItshmusContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Role>> GetAllAsync()
        {
            return await _context.Roles.ToListAsync();
        }

        public async Task<string?> GetRoleDescriptionByIdAsync(int id)
        {
            var roleDescription = await _context.Roles
              .Where(role => role.RoleId == id)
              .Select(role => role.RoleDescription)
              .FirstOrDefaultAsync();

            return roleDescription;
        }
    }
}