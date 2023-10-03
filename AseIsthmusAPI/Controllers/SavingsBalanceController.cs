using AseIsthmusAPI.Data.AseIsthmusModels;
using AseIsthmusAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AseIsthmusAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SavingsBalanceController : ControllerBase
    {
        private readonly SavingsBalanceService _service;

        public SavingsBalanceController(SavingsBalanceService service)
        {
            _service = service;
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<sp_GetSavingsBalance_Result>>> GetByUser([FromRoute] string id)
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
