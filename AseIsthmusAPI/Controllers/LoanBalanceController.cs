using AseIsthmusAPI.Data.AseIsthmusModels;
using AseIsthmusAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AseIsthmusAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoanBalanceController : ControllerBase
    {
        private readonly LoanBalanceService _service;

        public LoanBalanceController(LoanBalanceService service)
        {
            _service = service;
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<sp_GetLoanBalance_Result>>> GetByUser([FromRoute] string id)
        {
            var personIdClaim = User.FindFirst("PersonId")?.Value;
            if (personIdClaim != null && personIdClaim == id)
            {
                return Ok(await _service.GetBalanceByUser(id));
            }

            return Forbid();
        }
    }
}
