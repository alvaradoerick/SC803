using AseIsthmusAPI.Data.AseIsthmusModels;
using AseIsthmusAPI.Data.DTOs;
using AseIsthmusAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using System.Xml.XPath;


namespace AseIsthmusAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : Controller
    {
        private readonly LoginService _service;
        private IConfiguration config;

        public LoginController(LoginService service, IConfiguration config)
        {
            _service = service;
            this.config = config;
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> Login(LoginInDto loginDto)
        {
            try {
                var login = await _service.GetLogin(loginDto);
                if (login is null) 
                    return BadRequest("No se pudo procesar su solicitud.");

                else if (login.Pw != loginDto.Pw) 
                    return BadRequest(new { error = "Sus credenciales son inválidas." });

                else if (login.Person.IsActive is false) 
                    return BadRequest(new { error = "Su cuenta no está activada." });

                string jwtToken = await GenerateToken(login.Person);
                var responseDto = new AuthenticationResponseDto
                {
                    Token = jwtToken,
                    PersonId = login.Person.PersonId,
                    RoleId = login.Person.Role.RoleDescription
                };
                return Ok(responseDto);
            }
            catch (Exception)
            {
                return BadRequest("No se pudo procesar su solicitud.");
            }
          
        }

        private async Task<string> GenerateToken(User user)
        {
            
            var roleDescription = await _service.GetRoleDescription(user.RoleId);

            var claims = new[]
            {
        new Claim(ClaimTypes.Name, user.FirstName),
        new Claim(ClaimTypes.Email, user.EmailAddress),
        new Claim("RoleType", roleDescription),
        new Claim("PersonId", user.PersonId)
    }; 
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.GetSection("JWT:Key").Value));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var securityToken = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: creds);  

            string token = new JwtSecurityTokenHandler().WriteToken(securityToken);

            return token;
        }
       
    }
}
