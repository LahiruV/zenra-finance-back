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
        private string accessToken => Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        public FinanceController(IFinanceService service)
        {
            _service = service;
        }

        [HttpPost("AddFinance")]
        public async Task<IActionResult> AddFinance([FromBody] Finance finance)
        {
            var response = await _service.AddFinance(finance, accessToken);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("GetFinance")]
        public async Task<IActionResult> GetFinance()
        {
            var response = await _service.GetFinance(accessToken);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("GetThisMonthlyFinanceCount")]
        public async Task<IActionResult> GetThisMonthlyFinanceCount()
        {
            var response = await _service.GetThisMonthlyFinanceCount(accessToken);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("GetLastMonthlyFinanceCount")]
        public async Task<IActionResult> GetLastMonthlyFinanceCount()
        {
            var response = await _service.GetLastMonthlyFinanceCount(accessToken);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("GetThisYearFinanceCount")]
        public async Task<IActionResult> GetThisYearFinanceCount()
        {
            var response = await _service.GetThisYearFinanceCount(accessToken);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("GetLastYearFinanceCount")]
        public async Task<IActionResult> GetLastYearFinanceCount()
        {
            var response = await _service.GetLastYearFinanceCount(accessToken);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("GetFinanceByYear/{year}")]
        public async Task<IActionResult> GetFinanceByYear(int year)
        {
            var response = await _service.GetFinanceByYear(year, accessToken);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("GetCurrentWeekDailyFinanceCount")]
        public async Task<IActionResult> GetCurrentWeekDailyFinanceCount()
        {
            var response = await _service.GetCurrentWeekDailyFinanceCount(accessToken);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("GetAllFinancesCount")]
        public async Task<IActionResult> GetAllFinancesCount()
        {
            var response = await _service.GetAllFinancesCount(accessToken);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }


        [HttpPut("UpdateFinance/{id}")]
        public async Task<IActionResult> UpdateFinance(int id, [FromBody] Finance finance)
        {
            if (ModelState.IsValid)
            {
                var response = await _service.UpdateFinance(id, finance);
                if (response.IsSuccess)
                {
                    return Ok(response);
                }
                return BadRequest(response);
            }
            return BadRequest("Some properties are not valid");
        }

        [HttpDelete("DeleteFinance/{id}")]
        public async Task<IActionResult> DeleteFinance(int id)
        {
            var response = await _service.DeleteFinance(id);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
    }
}