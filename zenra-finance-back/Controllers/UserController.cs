using zenra_finance_back.Models;
using zenra_finance_back.Services.IServices;
using Microsoft.AspNetCore.Mvc;

namespace zenra_finance_back.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _service;
        public UserController(IUserService service)
        {
            _service = service;
        }

        [HttpPost("Register")]
       public async Task<IActionResult> Register([FromBody] User user)
        {
            if (ModelState.IsValid)
            {
                var response = await _service.Register(user);
                if (response.IsSuccess)
                {
                    return Ok(response);
                }
                return BadRequest(response);
            }
            return BadRequest("Some properties are not valid");
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            if (ModelState.IsValid)
            {
                var response = await _service.Login(loginRequest);
                if (response.IsSuccess)
                {
                    return Ok(response);  // Return token or success message
                }
                return BadRequest(response);  // Return failure message
            }
            return BadRequest("Invalid login request.");
        }

    }
}
