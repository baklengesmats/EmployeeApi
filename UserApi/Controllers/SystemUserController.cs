using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using UserApi.Common;
using UserApi.Repository.Model;
using UserApi.Services;

namespace UserApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SystemUserController : ControllerBase
    {
        private readonly ISystemUserService _userService;

        public SystemUserController(ISystemUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public IActionResult GetAllUsers()
        {
            var users = _userService.GetUsers();
            return Ok(users);
        }

        [HttpGet("active")]
        public IActionResult GetActiveUser()
        {
            var users = _userService.GetActiveUsers();
            return Ok(users);
        }

        [HttpPatch("deactive")]
        [Authorize]
        public IActionResult SoftDeleteUserById(int userId)
        {
            var result = _userService.SoftDeleteUser(userId);
            return ProcessOperationResult(result);
        }
        [HttpPatch("deactiveByMail")]
        [Authorize]
        public IActionResult SoftDeleteUserById(string mail)
        {
            var result = _userService.SoftDeleteUser(mail);
            return ProcessOperationResult(result);
        }

        [HttpDelete("delete/{userId}")]
        [Authorize(Roles = "Admin")]
        public IActionResult HardDeleteUserById(int userId)
        {
            var result = _userService.HardDeleteUser(userId);
            return ProcessOperationResult(result);
        }

        private IActionResult ProcessOperationResult(OperationResult result)
        {
            if (result.StatusCode == StatusCodes.Status404NotFound)
            {
                return NotFound(new { Message = "User not found" });
            }
            else if (result.Errors != null && result.Errors.Any())
            {
                return Problem(
                    title: "An unexpected error occurred.",
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }
            return NoContent();
        }
    }
}
