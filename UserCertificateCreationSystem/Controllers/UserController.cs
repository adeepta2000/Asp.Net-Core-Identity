using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using UserCertificateCreationSystem.Common;
using UserCertificateCreationSystem.Model;
using UserCertificateCreationSystem.Model.DBEntity;
using UserCertificateCreationSystem.Services;

namespace UserCertificateCreationSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IBaseService<User> _userService;

        public UserController(IBaseService<User> userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(BadRequestObjectResult), 400)]
        [ProducesResponseType(typeof(PayloadResponse<User>), 200)]
        [Route("{id:int}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var response = new PayloadResponse<User>
            {
                success = false,
                message = new List<string>() { "" },
                payload = null,
                operation_type = PayloadType.GetById
               
            };

            if (id <= 0)
            {
                response.message = new List<string>() { "Please enter valid id." };
                return BadRequest(response);
            }

            User data = await _userService.GetById(id);

            if (data == null)
            {
                response.success = false;
                response.message = new List<string>() { "No user found." };
                return NotFound(response);
            }
            response.success = true;
            response.message = new List<string>() { "Here is the user." };
            response.payload = data;
            return Ok(response);
        }

        [HttpGet]
        [ProducesResponseType(typeof(BadRequestObjectResult), 400)]
        [ProducesResponseType(typeof(PayloadResponse<List<User>>), 200)]
        [Route("")]
        public async Task<IActionResult> GetAllUser()
        {
            var response = new PayloadResponse<List<User>>
            {
                success = false,
                message = new List<string>() { "" },
                payload = null,
                operation_type = PayloadType.GetAllData
            };

            var users = await _userService.GetAll();

            if (users == null)
            {
                response.success = false;
                response.message = new List<string>() { "No User List found" };
                return NotFound(response);
            }
            response.success = true;
            response.message = new List<string>() { "Here is all users" };
            response.payload = users.ToList();
            return Ok(response);
        }

        [HttpPut]
        [ProducesResponseType(typeof(BadRequestObjectResult), 400)]
        [ProducesResponseType(typeof(PayloadResponse<User>), 200)]
        [Route("{id:int}")]
        public async Task<IActionResult> UpdateUser(int id, User model)
        {
            var response = new PayloadResponse<User>
            {
                success = false,
                message = new List<string>() { "" },
                payload = null,
                operation_type = PayloadType.Update
               
            };

            if (id <= 0)
            {
                response.message = new List<string>() { "Enter valid Id." };
                return BadRequest(response);
            }

            User oldData = await _userService.GetById(id);

            if (oldData == null)
            {
                response.success = false;
                response.message = new List<string>() { "No user found." };
                return NotFound(response);
            }

            oldData.Username = model.Username;
            oldData.Email = model.Email;
            oldData.Password = model.Password;

            OperationResult result = await _userService.Update(oldData);

            if (result.Success)
            {
                response.success = true;
                response.message = new List<string>() { "User updated successfully" };
                response.payload = result.Result;
                return Ok(response);
            }

            return BadRequest(response);

        }

        [HttpDelete]
        [ProducesResponseType(typeof(BadRequestObjectResult), 400)]
        [ProducesResponseType(typeof(PayloadResponse<User>), 200)]
        [Route("{id:int}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var response = new PayloadResponse<User>
            {
                success = false,
                message = new List<string>() { "" },
                payload = null,
                operation_type = PayloadType.Delete
            };

            if (id == 0)
            {
                response.message = new List<string>() { "Enter valid Id." };
                return BadRequest(response);
            }

            OperationResult result = _userService.Delete(id);

            if (result.Success)
            {
                response.success = true;
                response.message = new List<string>() { "User Successfully deleted." };
                response.payload = result.Result;
                return Ok(response);
            }

            return BadRequest(response);
        }





    }
}
