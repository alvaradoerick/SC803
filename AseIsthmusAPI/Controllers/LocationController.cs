using AseIsthmusAPI.Data.AseIsthmusModels;
using AseIsthmusAPI.Data.DTOs;
using AseIsthmusAPI.Services;
using AseIsthmusAPI.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections;

namespace AseIsthmusAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private readonly ILocationService _locationService;
        public LocationController(ILocationService locationService)
        {
            _locationService = locationService;
        }

        [HttpGet("provinces")]
        public async Task<ActionResult<List<Province>>> GetAllProvincesAsync()
        {
            try
            {
                var provinces = await _locationService.GetAllProvincesAsync();

                if (!provinces.Any())
                {
                    return NotFound("No se encontró ningúna provincia.");
                }

                return Ok(provinces);
            }
            catch (Exception)
            {
                return BadRequest("No se pudo procesar su solicitud.");
            }
        }

        [HttpGet("province/{provinceId}/cantons")]
        public async Task<ActionResult<List<Canton>>> GetCantonsByProvinceAsync(int provinceId)
        {
            try
            {
                var cantons = await _locationService.GetCantonsByProvinceAsync(provinceId);

                if (!cantons.Any())
                {
                    return NotFound("No se encontró ningún cantón.");
                }

                return Ok(cantons);
            }
            catch (Exception)
            {
                return BadRequest("No se pudo procesar su solicitud.");
            }
        }

        [HttpGet("canton/{cantonId}/districts")]
        public async Task<ActionResult<List<District>>> GetDistrictsByCantonAsync(int cantonId)
        {
            try
            {
                var districts = await _locationService.GetDistrictsByCantonAsync(cantonId);

                if (!districts.Any())
                {
                    return NotFound("No se encontró ningún distrito.");
                }

                return Ok(districts);
            }
            catch (Exception)
            {
                return BadRequest("No se pudo procesar su solicitud.");
            }                
        }

        [HttpGet("district/{districtId}")]
        public async Task<ActionResult<District>> GetDistrictInformationAsync([FromRoute] int districtId)
        {         
            try
            {
                var districtInfo = await _locationService.GetDistrictInformationAsync(districtId);

                if (districtInfo == null)
                {
                    return NotFound("No se encontró ningúna información del distrito.");
                }

                return Ok(districtInfo);
            }
            catch (Exception)
            {
                return BadRequest("No se pudo procesar su solicitud.");
            }       
        }
    }
}
