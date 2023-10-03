using AseIsthmusAPI.Data.DTOs;
using AseIsthmusAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AseIsthmusAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContributionBalanceController : ControllerBase
    {
        private readonly ContributionBalanceService _service;

        public ContributionBalanceController(ContributionBalanceService service)
        {
            _service = service;
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult> GetByUser([FromRoute] string id)
        {
            var personIdClaim = User.FindFirst("PersonId")?.Value;
            if (personIdClaim != null && personIdClaim == id)
            {
                var balance = await _service.GetByUser(id);
                if (balance is null)
                {
                    return NotFound(new { error = "No se pudo encontrar ningún balance con ese ID." });
                }
                else
                {
                    return Ok(balance);
                }
            }

            return Forbid();
        }
    }
}
