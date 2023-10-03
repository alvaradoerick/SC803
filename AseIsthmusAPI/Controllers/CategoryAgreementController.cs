using Microsoft.AspNetCore.Mvc;
using AseIsthmusAPI.Data.DTOs;
using AseIsthmusAPI.Services;
using System.Collections;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using AseIsthmusAPI.Data.AseIsthmusModels;
using AseIsthmusAPI.Data;
using AseIsthmusAPI.Services.Interfaces;
using AseIsthmusAPI.Repositories.Interfaces;

namespace AseIsthmusAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class CategoryAgreementController : ControllerBase
    {
        private readonly ICategoryAgreementsService _categoryAgreementsService;


        public CategoryAgreementController(ICategoryAgreementsService categoryAgreementsService)
        {
            _categoryAgreementsService = categoryAgreementsService;
        }

        #region Get

        [Authorize(Policy = "Administrator")]
        [HttpGet]
        public async Task<IEnumerable<CategoryAgreement>> Get()
        {
            return await _categoryAgreementsService.GetAllAsync();
        }

        [HttpGet("active-categories")]
        public async Task<IEnumerable<CategoryAgreement>> GetAllActiveCategories()
        {
            return await _categoryAgreementsService.GetAllActiveCategoriesAsync();
        }

        [HttpGet("categories-with-agreements")]
        public async Task<IEnumerable<sp_GetAllActiveCategoriesWithAgreements_Results>> GetAllActiveCategoriesWithAgreements()
        {
            return await _categoryAgreementsService.GetAllActiveCategoriesWithAgreementsAsync();
        }

        [Authorize(Policy = "Administrator")]
        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryAgreement>> GetById([FromRoute] int id)
        {
            var categoryAgreement = await _categoryAgreementsService.GetByIdAsync(id);

            if (categoryAgreement is null)
            {
                return NotFound(new { error = "No se pudo encontrar ninguna categoría." });
            }
            else
            {
                return categoryAgreement;
            }
        }

        #endregion

        #region Create

        [Authorize(Policy = "Administrator")]
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CategoryAgreement categoryAgreement)
        {
            var newCategoryAgreement = await _categoryAgreementsService.CreateAsync(categoryAgreement);

            return CreatedAtAction(nameof(GetById), new { id = newCategoryAgreement.CategoryAgreementId }, newCategoryAgreement);
        }
        #endregion

        #region Update

        [Authorize(Policy = "Administrator")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync([FromRoute] int id, [FromBody] CategoryAgreement categoryAgreement)
        {
            try
            {
                var categoryAgreementToUpdate = await _categoryAgreementsService.UpdateAsync(id, categoryAgreement);

                if (categoryAgreementToUpdate is not null)
                {

                    return NoContent();
                }
                else
                {
                    return NotFound(new { error = "No se encontró ninguna categoría con ese Id." });
                }
            }
            catch
            {
                return BadRequest(new { error = "No se pudo actualizar la categoría." });
            }
        }
        #endregion

        #region Delete
        [Authorize(Policy = "Administrator")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] int id)
        {

            bool hasAgreements = await _categoryAgreementsService.HasAgreementsAsync(id);

            if (hasAgreements)
            {
                return BadRequest(new { error = "No se puede eliminar la categoría porque tiene convenios asociados." });
            }

            else
            {
                var categoryAgreementToDelete = await _categoryAgreementsService.DeleteAsync(id);

                if (categoryAgreementToDelete)
                    return NoContent();

                return BadRequest(new { error = "No se pudo eliminar la categoría." });
            }
        }
        #endregion
    }
}
