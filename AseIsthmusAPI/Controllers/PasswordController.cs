using AseIsthmusAPI.Data.AseIsthmusModels;
using AseIsthmusAPI.Data.DTOs;
using AseIsthmusAPI.Services;
using AseIsthmusAPI.Templates;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using static Org.BouncyCastle.Math.EC.ECCurve;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace AseIsthmusAPI.Controllers
{

    [Route("api/[controller]/resetpassword")]
    [ApiController]
    public class PasswordController : ControllerBase
    {
        private readonly PasswordService _service;
        private IConfiguration config;


        public PasswordController(PasswordService service, IConfiguration config)
        {
            _service = service;
            this.config = config;
        }

        #region Authenticated update

        /// <summary>
        /// resets the password of user after logged in
        /// </summary>
        /// <param name="updatePasswordRequestDto"></param>
        /// <returns></returns>
        /// 
        [Authorize]
        [HttpPatch("{id}")]
        public async Task<IActionResult> ResetPasswordAuthenticated([FromRoute] string id, [FromBody] GeneratePasswordDto resetPassword)
        {
            if (resetPassword == null) return BadRequest(new { error = "La contraseña es requerida." });

            var newPasswordResponse = await _service.ResetPasswordAuthenticated(id, resetPassword);

            if (newPasswordResponse.Item1 != null) return Ok();

            return BadRequest(new { error = "Su solicitud no pudo enviarse." });
        }
        #endregion

        #region Unauthenticated Update
        /// <summary>
        /// resets the password of user without having to log in
        /// </summary>
        /// <param name="updatePasswordRequestDto"></param>
        /// <returns></returns>
        [HttpPatch]
        public async Task<IActionResult> ResetPasswordUnauthenticated([FromBody] GeneratePasswordDto generatePasswordDto)
        {         
            var newPassword = await _service.ResetPasswordUnauthenticated(generatePasswordDto);
            if (newPassword != null) {

                var newPasswordDto = new UpdatePasswordResponseDto
                {
                    NewPassword = newPassword,
                    EmailAddress = generatePasswordDto.EmailAddress
                };
               var tokenGenerated =  GeneratePasswordToken(newPasswordDto);
                return Ok(new { token = tokenGenerated.Result });
            }
           

            return BadRequest(new { error = "Su solicitud no pudo enviarse." });
        }

        #endregion


        private async Task<string> GeneratePasswordToken(UpdatePasswordResponseDto updatePasswordResponseDto)
        {
            var claims = new[]
             {
                new Claim("NewPassword", updatePasswordResponseDto.NewPassword),
                new Claim("EmailAddress", updatePasswordResponseDto.EmailAddress),
                new Claim(JwtRegisteredClaimNames.Exp, DateTimeOffset.UtcNow.AddMinutes(60).ToUnixTimeSeconds().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.GetSection("JWT:Key").Value));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var securityToken = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(60),
                signingCredentials: creds);

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.WriteToken(securityToken);

            return token;
        }
    }
}
