using AseIsthmusAPI.Data.AseIsthmusModels;
using AseIsthmusAPI.Data.DTOs;
using AseIsthmusAPI.Services;
using AseIsthmusAPI.Templates;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace AseIsthmusAPI.Controllers
{
    [Route("api/users/{id}/[controller]")]
    [ApiController]
    public class BeneficiaryController : ControllerBase
    {
        private readonly BeneficiaryService _service;

        #region Constructor
        public BeneficiaryController(BeneficiaryService service)
        {
            _service = service;
        }

        #endregion

        #region Get

        [Authorize]
        [HttpGet]
        public async Task<IEnumerable<Beneficiary>?> Get([FromRoute] string id) {

            return await _service.GetBeneficiariesByPersonId(id);
        }
        #endregion

        #region Create
        [HttpPost]
        public async Task<IActionResult> Insert([FromRoute] string id, List<BeneficiaryDtoIn> beneficiaries)
        {
            if (beneficiaries == null || beneficiaries.Count == 0)
            {
                return BadRequest("No se pudo procesar su solicitud.");
            }
            try
            {
                foreach (var beneficiary in beneficiaries)
                {
                    var newUser = await _service.Create(id, beneficiary);
                }
                return Ok("Los beneficiarios fueron agregados correctamente.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        #endregion

        #region Delete
        /// <summary>
        /// deletes all the beneficiaries
        /// </summary>
        /// <param name="updatePasswordRequestDto"></param>
        /// <returns></returns>

        [Authorize(Policy = "Administrator")]
        [HttpDelete]
        public async Task<IActionResult> Delete([FromRoute] string id)
        {
                var beneficiaries = await _service.GetBeneficiariesByPersonId(id);

                if (beneficiaries is not null)
                {
                    var result = await _service.DeleteUser(id);
                    if (string.IsNullOrEmpty(result))
                    {
                        return NoContent();
                    }
                    else
                    {
                        return BadRequest(new { error = result });
                    }
                }
                else
                {
                    return NotFound();
                }
            }

        #endregion
    }
}
