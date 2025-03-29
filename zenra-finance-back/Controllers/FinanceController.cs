using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using zenra_finance_back.Models;
using zenra_finance_back.Services.IServices;

namespace zenra_finance_back.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FinanceController : ControllerBase
    {
        private readonly IFinanceService _service;
        public FinanceController(IFinanceService service)
        {
            _service = service;
        }

        [HttpPost("AddFinance")]
        public async Task<IActionResult> AddFinance([FromBody] Finance finance)
        {
            if (ModelState.IsValid)
            {
                var response = await _service.AddFinance(finance);
                if (response.IsSuccess)
                {
                    return Ok(response);
                }
                return BadRequest(response);
            }
            return BadRequest("Some properties are not valid");
        }
    }
}