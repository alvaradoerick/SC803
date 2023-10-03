using AseIsthmusAPI.Data.AseIsthmusModels;

namespace AseIsthmusAPI.Repositories.Interfaces
{
    public interface IRoleRepository
    {
        Task<IEnumerable<Role>> GetAllAsync();
        Task<string?> GetRoleDescriptionByIdAsync(int id);
    }
}
