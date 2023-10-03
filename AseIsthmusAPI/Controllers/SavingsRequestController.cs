using AseIsthmusAPI.Data.AseIsthmusModels;
using AseIsthmusAPI.Data.DTOs;
using AseIsthmusAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Crypto.Agreement;

namespace AseIsthmusAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SavingsRequestController : ControllerBase
    {
        private readonly SavingsRequestService _service;

        public SavingsRequestController(SavingsRequestService service)
        {
            _service = service;
        }

        #region Get

        [Authorize(Policy = "Administrator")]
        [HttpGet]
        public async Task<IEnumerable<SavingsRequestDataDto>?> Get()
        {
            return await _service.GetAll();
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<SavingsRequestDataDto>> GetById([FromRoute] int id)
        {
            var savings = await _service.GetById(id);

            if (savings is null)
            {
                return NotFound(new { error = "No se pudo encontrar ningun ahorro con ese ID." });
            }
            else
            {
                return savings;
            }
        }

        [Authorize]
        [HttpGet("employee/{id}")]
        public async Task<ActionResult<IEnumerable<SavingsRequestDataDto>>> GetByEmployee([FromRoute] string id)
        {
            var personIdClaim = User.FindFirst("PersonId")?.Value;
            if (personIdClaim != null && personIdClaim == id)
            {
                var savings = await _service.GetByEmployee(id);
                if (savings is null)
                {
                    return NotFound(new { error = "No se pudo encontrar ningún préstamo con ese ID." });
                }
                else
                {
                    return Ok(savings);
                }
            }

            return Forbid();
        }

       
        #endregion

        #region Create
        [Authorize]
        [HttpPost("{id}")]
        public async Task<IActionResult> Create([FromRoute]string id, ManageSavingsRequestDto savings)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                await _service.Create(id, savings);
                return Ok(savings);
            }
        }
        #endregion

        #region Update

        [Authorize(Policy = "Administrator")]
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateSavings([FromRoute] int id, [FromBody] ManageSavingsRequestByAdminDto savings)
        {
            var savingToUpdate = await _service.UpdateSaving(id, savings);

            if (savingToUpdate is not null)
            {
                return NoContent();
            }
            else
            {
                return NotFound(new { error = "No se pudo actualizar el ahorro." });
            }
        }


        [Authorize(Policy = "Administrator")]
        [HttpPatch("cancel-savings/{id}")]
        public async Task<IActionResult> CancelSavings([FromRoute] int id, [FromBody] CancelSavingsRequestDto cancelSavings)
        {
            var savingToUpdate = await _service.CancelSavings(id, cancelSavings);

            if (savingToUpdate is not null)
            {
                return NoContent();
            }
            else
            {
                return NotFound(new { error = "No se pudo cancelar el ahorro." });
            }
        }
        #endregion

        #region Delete

        [Authorize(Policy = "Administrator")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete([FromRoute] int id)
        {
            try
            {
                await _service.Delete(id);
                return NoContent();
            }
            catch (Exception)
            {
                return BadRequest(new { error = "No se pudo eliminar el ahorro ya que hay un balance asociado." });
            }
        }
        #endregion
    }
}
