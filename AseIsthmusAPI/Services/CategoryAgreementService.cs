using Microsoft.EntityFrameworkCore;
using AseIsthmusAPI.Data;
using AseIsthmusAPI.Data.AseIsthmusModels;
using System.Net;
using AseIsthmusAPI.Services.Interfaces;
using AseIsthmusAPI.Repositories.Interfaces;

namespace AseIsthmusAPI.Services
{
    public class CategoryAgreementService : ICategoryAgreementsService
    {
        private readonly ICategoryAgreementsRepository _categoryAgreementsRepository;
        public CategoryAgreementService(ICategoryAgreementsRepository categoryAgreementsRepository)
        {
            _categoryAgreementsRepository = categoryAgreementsRepository;
        }
        public async Task<CategoryAgreement> CreateAsync(CategoryAgreement newCategoryAgreement)
        {
            return await _categoryAgreementsRepository.CreateAsync(newCategoryAgreement);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _categoryAgreementsRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<CategoryAgreement>> GetAllActiveCategoriesAsync()
        {
            return await _categoryAgreementsRepository.GetAllActiveCategoriesAsync();
        }

        public async Task<IEnumerable<sp_GetAllActiveCategoriesWithAgreements_Results>> GetAllActiveCategoriesWithAgreementsAsync()
        {
            return await _categoryAgreementsRepository.GetAllActiveCategoriesWithAgreementsAsync();
        }

        public async Task<IEnumerable<CategoryAgreement>> GetAllAsync()
        {
            return await _categoryAgreementsRepository.GetAllAsync();
        }

        public async Task<CategoryAgreement?> GetByIdAsync(int id)
        {
            return await _categoryAgreementsRepository.GetByIdAsync(id);
        }

        public async Task<bool> HasAgreementsAsync(int id)
        {
            return await _categoryAgreementsRepository.HasAgreementsAsync(id);
        }

        public async Task<CategoryAgreement> UpdateAsync(int id, CategoryAgreement categoryAgreement)
        {
            return await _categoryAgreementsRepository.UpdateAsync(id, categoryAgreement);
        }
    }
}
