using AseIsthmusAPI.Data.AseIsthmusModels;
using AseIsthmusAPI.Data.DTOs;
using AseIsthmusAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AseIsthmusAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoansTypeController : ControllerBase
    {
        private readonly LoansTypeService _service;

        public LoansTypeController(LoansTypeService service)
        {
            _service = service;
        }

        #region Get
        [Authorize(Policy = "Administrator")]
        [HttpGet]
        public async Task<IEnumerable<LoansType>> Get()
        {
            return await _service.GetAll();
        }

        [Authorize]
        [HttpGet("active-loans")]
        public async Task<IEnumerable<LoansType>> GetAllActiveLoanTypes()
        {
            return await _service.GetAllActiveLoans();
        }

        [Authorize(Policy = "Administrator")]
        [HttpGet("{id}")]
        public async Task<ActionResult<LoanTypeDataDto>> GetById([FromRoute] int id)
        {
            var loanType = await _service.GetById(id);

            if (loanType is null)
            {
                return NotFound(new { error = "No se pudo encontrar ningun tipo de préstamo." });
            }
            else
            {
                return loanType;
            }
        }

        #endregion

        #region Create

        [Authorize(Policy = "Administrator")]
        [HttpPost]
        public async Task<IActionResult> Create(ManageLoanTypeDto loanType)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                await _service.Create(loanType);
                return Ok(loanType);
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
                return BadRequest(new { error = "No se pudo eliminar el tipo de préstamo." });
            }


        }

        #endregion

        #region Update
        [Authorize(Policy = "Administrator")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] ManageLoanTypeDto loanType)
        {
            var loanTypeToUpdate = await _service.Update(id, loanType);

            if (loanTypeToUpdate is not null)
            {
                return NoContent();
            }
            else
            {
                return NotFound(new { error = "No se pudo actualizar el tipo de préstamo." });
            }
        }
        #endregion
    }
}