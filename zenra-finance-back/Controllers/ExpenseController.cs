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
    public class ExpenseController : ControllerBase
    {
        private readonly IExpenseService _service;
        private string accessToken => Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        public ExpenseController(IExpenseService service)
        {
            _service = service;
        }

        [HttpPost("AddExpense")]
        public async Task<IActionResult> AddExpense([FromBody] Expense expense)
        {
            var response = await _service.AddExpense(expense, accessToken);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("GetExpense")]
        public async Task<IActionResult> GetExpense()
        {
            var response = await _service.GetExpense(accessToken);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("GetThisMonthlyExpensesCount")]
        public async Task<IActionResult> GetThisMonthlyExpensesCount()
        {
            var response = await _service.GetThisMonthlyExpensesCount(accessToken);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("GetTodayExpensesCount")]
        public async Task<IActionResult> GetTodayExpensesCount()
        {
            var response = await _service.GetTodayExpensesCount(accessToken);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("GetAllExpensesCount")]
        public async Task<IActionResult> GetAllExpensesCount()
        {
            var response = await _service.GetAllExpensesCount(accessToken);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("GetCurrentWeekDailyExpenseCount")]
        public async Task<IActionResult> GetCurrentWeekDailyExpenseCount()
        {
            var response = await _service.GetCurrentWeekDailyExpenseCount(accessToken);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("GetExpenseByYear/{year}")]
        public async Task<IActionResult> GetExpenseByYear(int year)
        {
            var response = await _service.GetExpenseeByYear(year, accessToken);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("GetCurrentWeekDailyIncomeExpenseCount")]
        public async Task<IActionResult> GetCurrentWeekDailyIncomeExpenseCount()
        {
            var response = await _service.GetCurrentWeekDailyIncomeExpenseCount(accessToken);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("GetIncomeExpenseByYear/{year}")]
        public async Task<IActionResult> GetIncomeExpenseByYear(int year)
        {
            var response = await _service.GetIncomeExpenseeByYear(year, accessToken);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

    }
}