using AseIsthmusAPI.Data;
using AseIsthmusAPI.Data.AseIsthmusModels;
using AseIsthmusAPI.Repositories.Interfaces;


namespace AseIsthmusAPI.Repositories
{
    public class CategoryAgreementsRepository : ICategoryAgreementsRepository
    {
        private readonly AseItshmusContext _context;

        public CategoryAgreementsRepository(AseItshmusContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<CategoryAgreement>> GetAllAsync()
        {
            return await _context.CategoryAgreements.ToListAsync();
        }
        public async Task<CategoryAgreement?> GetByIdAsync(int id)
        {
           var category =  await _context.CategoryAgreements.FindAsync(id);
            
            if (category is not null)  return category;
            
            return null;
        }
        public async Task<IEnumerable<CategoryAgreement>> GetAllActiveCategoriesAsync()
        {
            var results = await _context.CategoryAgreements
                .Where(ca => ca.IsActive == true)
                      .ToListAsync();

            return results.AsEnumerable();
        }
        public async Task<IEnumerable<sp_GetAllActiveCategoriesWithAgreements_Results>> GetAllActiveCategoriesWithAgreementsAsync()
        {
            var results = await _context.Sp_GetAllActiveCategoriesWithAgreements
                      .FromSqlRaw("EXEC sp_GetAllActiveCategoriesWithAgreements")
                      .ToListAsync();

            return results.AsEnumerable();
        }
        public async Task<CategoryAgreement> CreateAsync(CategoryAgreement newCategoryAgreement)
        {
            _context.CategoryAgreements.Add(newCategoryAgreement);
            await _context.SaveChangesAsync();

            return newCategoryAgreement;
        }
        public async Task<CategoryAgreement> UpdateAsync(int id, CategoryAgreement categoryAgreement)
        {
            var existingCategoryAgreement = await GetByIdAsync(id);

            if (existingCategoryAgreement is not null)
            {
                existingCategoryAgreement.Description = categoryAgreement.Description;
                existingCategoryAgreement.IsActive = categoryAgreement.IsActive;

                await _context.SaveChangesAsync();
                return categoryAgreement;
            }
            return null;
        }
        public async Task<bool> DeleteAsync(int id)
        {
            var categoryAgreementToDelete = await GetByIdAsync(id);

            if (categoryAgreementToDelete is not null)
            {
                _context.CategoryAgreements.Remove(categoryAgreementToDelete);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> HasAgreementsAsync(int id)
        {
            var agreementsCount = await _context.Agreements.Where(a => a.CategoryAgreementId == id).CountAsync();

            return agreementsCount > 0;
        }
    }
}