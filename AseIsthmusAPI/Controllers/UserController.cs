using AseIsthmusAPI.Data;
using AseIsthmusAPI.Data.AseIsthmusModels;
using AseIsthmusAPI.Data.DTOs;
using AseIsthmusAPI.Services;
using AseIsthmusAPI.Templates;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AseIsthmusAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _service;

        #region Constructors 

        public UserController(UserService service)
        {
            _service = service;
        }

        #endregion

        #region Get
        [Authorize(Policy = "Administrator")]
        [HttpGet]
        public async Task<IEnumerable<GetUserInformation>?> Get()
        {
            return await _service.GetAll();
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<GetUserInformation>> GetById([FromRoute] string id)
        {
            var user = await _service.GetDtoById(id);

            if (user is null) return UserNotFound(id);

            return user;
        }

        #endregion

        #region Create

        [HttpPost]

        public async Task<IActionResult> Insert(InsertUser user)
        {
            string validationRecord = await _service.DuplicateAccount(null, user.PersonId, user.NumberId, user.EmailAddress);

            if (!validationRecord.Equals("Valid"))
                return BadRequest(new { error = $"{validationRecord}" });        

            if (!ModelState.IsValid) return BadRequest(new { errors = ModelState });

            var newUser = await _service.Create(user);

            return CreatedAtAction(nameof(GetById), new { id = newUser.PersonId }, newUser);

        }

        #endregion

        #region Patch
        /// <summary>
        /// Update user by Admin
        /// </summary>
        /// <param name="id"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        [Authorize(Policy = "Administrator")]
        [HttpPatch("employee/{id}")]
        public async Task<IActionResult> Update([FromRoute] string id, UpdateUserByAdmin user)
        {
            string validationRecord = await _service.DuplicateAccount(id, user.PersonId, user.NumberId, user.EmailAddress);

            if (!validationRecord.Equals("Valid"))
                return BadRequest(new { error = $"{validationRecord}" });
            try
            {
                var userExist = await _service.UpdateUserByAdmin(id, user);
               
                if (userExist) return Ok(new { message = "Solicitud exitosa" });

                    return UserNotFound(id);
            }
            catch (ArgumentException)
            {
                return BadRequest(new { error = "No se pudo procesar su solicitud." });
            }
        }

        /// <summary>
        /// update user by user
        /// </summary>
        /// <param name="id"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateUserByUser([FromRoute] string id, UpdateUserByUser user)
        {
            try
            {
                var exists = await _service.UpdateUserByUser(id, user);

                if (exists) return Ok(new { message = "Sus cambios han sido guardados exitosamente." });

                return UserNotFound(id);
            }
            catch (ArgumentException)
            {
                return BadRequest(new { error = "No se pudo procesar su pedido." });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { error = "Su sesión ha expirado." }); 
            }
        }
        #endregion

        #region Delete

        [Authorize(Policy = "Administrator")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _service.DeleteUser(id);

            if (string.IsNullOrEmpty(result))
            {
                return NoContent();
            }
            else
            {
                return BadRequest(new { error = result });
            }
        }

        #endregion

        #region Patch user status

        [Authorize(Policy = "Administrator")]
        [HttpPatch("activateuser/{id}")]
        public async Task<IActionResult> ManageUserStatus([FromRoute] string id)
        {             
            var result = await _service.ManageUserStatus(id);

            if (result is not null)  return Ok(result);

            return BadRequest(new { error = "No se pudo procesar su solicitud." });

        }

        #endregion

        #region Non Actions
        [NonAction]
        public NotFoundObjectResult UserNotFound(string id)
        {
            return NotFound(new { error = $"El usuario con código '{id}' no existe." });
        }

        #endregion
    }
}
