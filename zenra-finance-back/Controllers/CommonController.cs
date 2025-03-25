using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using zenra_finance_back.Services.IServices;

namespace zenra_finance_back.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommonController : ControllerBase
    {
        private readonly ICommonService _service;
        public CommonController(ICommonService service)
        {
            _service = service;
        }

        [HttpPost("ConvertImageToBase64")]
        public async Task<IActionResult> ConvertImageToBase64([FromForm] IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                var response = await _service.ConvertImageToBase64Async(imageFile);
                return Ok(response);
            }
            return BadRequest("Some properties are not valid");
        }
    }
}
