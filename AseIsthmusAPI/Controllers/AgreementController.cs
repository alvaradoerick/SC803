using Microsoft.AspNetCore.Mvc;
using AseIsthmusAPI.Data;
using AseIsthmusAPI.Data.DTOs;
using AseIsthmusAPI.Services;
using System.Collections;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using AseIsthmusAPI.Data.AseIsthmusModels;
using AseIsthmusAPI.Templates;
using DocumentFormat.OpenXml.Spreadsheet;

namespace AseIsthmusAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AgreementController : ControllerBase
    {
        private readonly AgreementService _service;
        private readonly EmailService _emailService;
        private readonly UserService _userService;

        public AgreementController(AgreementService service, EmailService emailService, UserService userService)
        {
            _service = service;
            _emailService = emailService;
            _userService = userService;
        }

        #region Get

        [HttpGet]
        public async Task<IEnumerable<AgreementDataDto>?> Get()
        {
            return await _service.GetAll();
        }

        [HttpGet("active-agreements")]
        public async Task<IEnumerable<sp_GetAllActiveAgreements_Result>> GetAllActiveAgreements()
        {

            return await _service.GetAllActiveAgreements();
        }

        [Authorize(Policy = "Administrator")]
        [HttpGet("{id}")]
        public async Task<ActionResult<AgreementDataDto>> GetById([FromRoute] int id)
        {
            var agreementData = await _service.GetById(id);

            if (agreementData is null)
            {
                return NotFound(new { error = "No se pudo encontrar ningun convenio." });
            }
            else
            {
                HtmlContentProvider emailTemplate = new HtmlContentProvider();
                return Ok(agreementData);
            }
        }

        #endregion

        #region Create

        [Authorize(Policy = "Administrator")]
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] ManageAgreementDto agreementData)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                await _service.Create(agreementData);
                return Ok(agreementData);
            }
        }
        #endregion

        #region Delete
        [Authorize(Policy = "Administrator")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete([FromRoute] int id)
        {
            try {
                await _service.Delete(id);
                return NoContent();
            } catch (Exception) {
                return BadRequest(new { error = "No se pudo eliminar el convenio." });
            }
                
        }

        #endregion

        #region Update
        [Authorize(Policy = "Administrator")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromForm] ManageAgreementDto agreementData)
        {
            var agreementToUpdate = await _service.Update(id, agreementData);

            if (agreementToUpdate)
            {               
                return NoContent();
            }
            else
            {
                return NotFound(new { error = "No se pudo actualizar el convenio." });
            }
        }
        #endregion
    }
}
