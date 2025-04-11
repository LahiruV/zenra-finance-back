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
        public ExpenseController(IExpenseService service)
        {
            _service = service;
        }

        [HttpPost("AddExpense")]
        public async Task<IActionResult> AddExpense([FromBody] Expense expense)
        {
            if (ModelState.IsValid)
            {
                var response = await _service.AddExpense(expense);
                if (response.IsSuccess)
                {
                    return Ok(response);
                }
                return BadRequest(response);
            }
            return BadRequest("Some properties are not valid");
        }

        [HttpGet("GetExpense")]
        public async Task<IActionResult> GetExpense()
        {
            var response = await _service.GetExpense();
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("GetThisMonthlyExpensesCount")]
        public async Task<IActionResult> GetThisMonthlyExpensesCount()
        {
            var response = await _service.GetThisMonthlyExpensesCount();
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("GetTodayExpensesCount")]
        public async Task<IActionResult> GetTodayExpensesCount()
        {
            var response = await _service.GetTodayExpensesCount();
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("GetAllExpensesCount")]
        public async Task<IActionResult> GetAllExpensesCount()
        {
            var response = await _service.GetAllExpensesCount();
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("GetCurrentWeekDailyExpenseCount")]
        public async Task<IActionResult> GetCurrentWeekDailyExpenseCount()
        {
            var response = await _service.GetCurrentWeekDailyExpenseCount();
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("GetExpenseByYear/{year}")]
        public async Task<IActionResult> GetExpenseByYear(int year)
        {
            var response = await _service.GetExpenseeByYear(year);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("GetCurrentWeekDailyIncomeExpenseCount")]
        public async Task<IActionResult> GetCurrentWeekDailyIncomeExpenseCount()
        {
            var response = await _service.GetCurrentWeekDailyIncomeExpenseCount();
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("GetMonthlyIncomeExpenseCount")]
        public async Task<IActionResult> GetMonthlyIncomeExpenseCount()
        {
            var response = await _service.GetMonthlyIncomeExpenseCount();
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

    }
}