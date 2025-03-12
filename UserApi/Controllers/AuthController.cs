using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserApi.Services;

namespace UserApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _userAuthService;
       
        public AuthController(IAuthService userAuthService)
        {
            _userAuthService = userAuthService;
        }

        [HttpPost]
        public IActionResult Login(string mail, string password)
        {
            var validCredentials = _userAuthService.ValidateLogin(mail, password);
            if (!validCredentials)
            {
                return Unauthorized();
            }
            var tokenString = _userAuthService.GenerateJwtToken(mail);
            return Ok(new { Token = tokenString });
        }
    }
}
