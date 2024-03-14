using BaseLibrary.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServerLibrary.Repositories.Contracts;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthenticationController : ControllerBase
    {
        private readonly IUserAccount _userAccount; // Use dependency injection

        public AuthenticationController(IUserAccount userAccount)
        {
            _userAccount = userAccount; // Assign injected dependency
        }

        //Controller For Register the New User
        [HttpPost("register")]
        public async Task<IActionResult> CreateAsync(Register user)
        {
            if (user == null)
            {
                return BadRequest(new { message = "Model is empty" }); 
            }
            var result = await _userAccount.CreateAsync(user);
            return Ok(result);
        }

        //Controller For Signing User
        [HttpPost("login")]
        public async Task<IActionResult> SignInAsync(Login user)
        {
            if (user == null) 
            {
                return BadRequest(new { message = "Model is empty" });
            }
            var results = await _userAccount.SignInAsync(user);
            return Ok(results);
        }

        //Controller for refreshToken
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshTokenAsync(RefreshToken token)
        {
            if (token == null)
            {
                return BadRequest(new { message = "Model is empty" });
            }
            var results = await _userAccount.RefreshTokenAsync(token);
            return Ok(results);
        }
    }
}
