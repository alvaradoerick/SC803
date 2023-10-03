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
    public class ContributionUsageController : ControllerBase
    {
        private readonly ContributionUsageService _service;

        public ContributionUsageController(ContributionUsageService service)
        {
            _service = service;
        }

        #region Get

        
        [HttpGet]
        public async Task<IEnumerable<ContributionUsage>> Get()
        {
            return await _service.GetAll();
        }
        #endregion
    }
}
