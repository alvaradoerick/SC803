using AseIsthmusAPI.Data.AseIsthmusModels;
using AseIsthmusAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AseIsthmusAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SavingsTypeController : ControllerBase
    {
        private readonly SavingsTypeService _service;


        public SavingsTypeController(SavingsTypeService service)
        {
            _service = service;
        }

        #region Get
        [Authorize(Policy = "Administrator")]
        [HttpGet]
        public async Task<IEnumerable<SavingsType>> Get()
        {
            return await _service.GetAll();
        }

        [Authorize]
        [HttpGet("active-savings")]
        public async Task<IEnumerable<SavingsType>> GetAllActiveSavings()
        {
            return await _service.GetAllActiveSavings();
        }

        [Authorize(Policy = "Administrator")]
        [HttpGet("{id}")]
        public async Task<ActionResult<SavingsType>> GetById([FromRoute] int id)
        {
            var savingsType = await _service.GetById(id);

            if (savingsType is null)
            {
                return NotFound(new { error = "No se pudo encontrar ningun ahorro." });
            }
            else
            {
                return savingsType;
            }
        }

        #endregion

        #region Create
        [Authorize(Policy = "Administrator")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SavingsType savingsType)
        {
            var newSavingsType = await _service.Create(savingsType);

            return CreatedAtAction(nameof(GetById), new { id = newSavingsType.SavingsTypeId }, newSavingsType);
        }
        #endregion

        #region Update

        [Authorize(Policy = "Administrator")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] SavingsType savingsType)
        {
            var savingsTypeToUpdate = await _service.GetById(id);

            if (savingsTypeToUpdate is not null)
            {
                await _service.Update(id, savingsType);
                return NoContent();
            }
            else
            {
                return NotFound(new { error = "No se pudo actualizar el tipo de ahorro." });
            }
        }
        #endregion

        #region Delete

        [Authorize(Policy = "Administrator")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {

            bool hasSavings = await _service.HasSavings(id);

            if (hasSavings)
            {
                return BadRequest(new { error = "No se puede eliminar el tipo de ahorro porque tiene solicitudes asociadas." });
            }
            else
            {
                var savingsTypeToDelete = await _service.GetById(id);

                if (savingsTypeToDelete is not null)
                {
                    await _service.Delete(id);
                    return NoContent();
                }
                else
                {
                    return NotFound();
                }
            }
        }
        #endregion
    }
}
