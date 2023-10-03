using AseIsthmusAPI.Data;
using AseIsthmusAPI.Data.AseIsthmusModels;
using AseIsthmusAPI.Data.DTOs;
using AseIsthmusAPI.Services;
using AseIsthmusAPI.Templates;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static AseIsthmusAPI.Data.DTOs.UpdateLoanRequestRespondReviewDto;

namespace AseIsthmusAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoanRequestController : ControllerBase
    {
        private readonly LoanRequestService _service;

        public LoanRequestController(LoanRequestService service)
        {
            _service = service;

        }
        #region Get

        [Authorize(Policy = "Loan-Approvers")]
        [HttpGet]
        public async Task<IEnumerable<LoanRequestDataDto>?> Get()
        {
            return await _service.GetAll();
        }


        [Authorize(Policy = "Loan-Approvers")]
        [HttpGet("{id}")]
        public async Task<ActionResult<LoanRequestDataDto>> GetById([FromRoute] int id)
        {
            var loan = await _service.GetById(id);

            if (loan is null) return NotFound(new { error = "No se pudo encontrar ningun préstamo con ese ID." });

            return loan;
        }

        [Authorize]
        [HttpGet("employee/{id}")]
        public async Task<ActionResult<IEnumerable<LoanRequestDataDto>>> GetByEmployee([FromRoute] string id)
        {
            var personIdClaim = User.FindFirst("PersonId")?.Value;
            if (personIdClaim != null && personIdClaim == id)
            {
                var loan = await _service.GetByEmployee(id);

                if (loan is null) return NotFound(new { error = "No se pudo encontrar ningún préstamo con ese ID." });

                return Ok(loan);
            }

            return Forbid();
        }

        #endregion

        #region Create

        [Authorize]
        [HttpPost("{id}")]
    public async Task<IActionResult> Create([FromRoute] string id, ManageLoanRequestDto loan)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        else
        {
            await _service.Create(id, loan);
            return Ok(loan);
        }
    }
        #endregion

        #region Update

        [Authorize(Policy = "Loan-Approvers")]
        [HttpPatch("{id}")]
        public async Task<IActionResult> ApproveLoan([FromRoute] int id, [FromBody] UpdateLoanRequestByAdminDto loan)
        {
            var loanToUpdate = await _service.ApproveLoan(id, loan);

            if (loanToUpdate.Item1 is not null) return NoContent();

            return NotFound(new { error = "No se pudo actualizar el préstamo." });
        }


        [Authorize(Policy = "Administrator")]
        [HttpPatch("request-review/{id}")]
        public async Task<ActionResult> RequestLoanReview([FromRoute] int id, [FromBody] UpdateLoanRequestRequestReviewDto requestReviewComments)
        {
            var requestSent = await _service.RequestLoanReview(id, requestReviewComments); 

            if (!requestSent) return NotFound(new { error = "No se pudo completar su solicitud." });
       
             return NoContent();
        }

        [Authorize(Policy = "Loan-Approvers")]
        [HttpPatch("respond-review/{id}")]
        public async Task<ActionResult> RespondLoanReview([FromRoute] int id, [FromBody] UpdateLoanRequestRespondReviewDto reviewRespond)
        {
            var requestSent = await _service.RespondLoanReview(id, reviewRespond);

            if (!requestSent) return NotFound(new { error = "No se pudo completar su solicitud." });

             return NoContent();    
        }
        #endregion

        #region methods

        [HttpPost("calculation")]
        public async Task<IActionResult> GetLoanCalculation([FromBody] LoanCalculationType loanCalculation)
        {
            var result = await _service.GetLoanCalculation(loanCalculation);

            return Ok(result);
        }

        #endregion

        #region Delete

        [Authorize(Policy = "Loan-Approvers")]
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
                return BadRequest(new { error = "No se pudo eliminar el préstamo ya que hay un balance asociado." });
            }
        }
        #endregion
    }
}
