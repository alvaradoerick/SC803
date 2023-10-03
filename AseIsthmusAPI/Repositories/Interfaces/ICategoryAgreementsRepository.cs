using AseIsthmusAPI.Data.AseIsthmusModels;

namespace AseIsthmusAPI.Repositories.Interfaces
{
    public interface ICategoryAgreementsRepository
    {
        public Task<IEnumerable<CategoryAgreement>> GetAllAsync();
        public Task<CategoryAgreement?> GetByIdAsync(int id);
        public Task<IEnumerable<CategoryAgreement>> GetAllActiveCategoriesAsync();
        public Task<IEnumerable<sp_GetAllActiveCategoriesWithAgreements_Results>> GetAllActiveCategoriesWithAgreementsAsync();
        public Task<CategoryAgreement> CreateAsync(CategoryAgreement newCategoryAgreement);
        public Task<CategoryAgreement> UpdateAsync(int id, CategoryAgreement categoryAgreement);
        public Task<bool> DeleteAsync(int id);
        public Task<bool> HasAgreementsAsync(int id);
    }
}
