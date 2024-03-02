using BaseLibrary.DTOs; 
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServerLibrary.Repositories.Contracts;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IUserAccount _userAccount; // Use dependency injection

        public AuthenticationController(IUserAccount userAccount)
        {
            _userAccount = userAccount; // Assign injected dependency
        }

        //Method For Register the New User
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
    }
}
