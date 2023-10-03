using AseIsthmusAPI.Data.AseIsthmusModels;

namespace AseIsthmusAPI.Services.Interfaces
{
    public interface IRoleService
    {
        Task<IEnumerable<Role>> GetAllAsync();
        Task<string> GetRoleDescriptionByIdAsync(int id);
    }
}
