using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using zenra_finance_back.Models;

namespace zenra_finance_back.Services.IServices
{
    public interface IExpenseService
    {
        Task<Response<Expense>> AddExpense(Expense expense, string accessToken);
        Task<Response<List<Expense>>> GetExpense(string accessToken);
        Task<Response<MonthExpenseResponse>> GetThisMonthlyExpensesCount(string accessToken);
        Task<Response<decimal>> GetTodayExpensesCount(string accessToken);
        Task<Response<decimal>> GetAllExpensesCount(string accessToken);
        Task<Response<List<CurrentWeekDailyExpenseResponse>>> GetCurrentWeekDailyExpenseCount(string accessToken);
        Task<Response<List<MonthExpenseResponse>>> GetExpenseeByYear(int year, string accessToken);
        Task<Response<List<CurrentWeekDailyIncomeExpenseResponse>>> GetCurrentWeekDailyIncomeExpenseCount(string accessToken);
        Task<Response<List<MonthIncomeExpenseResponse>>> GetIncomeExpenseeByYear(int year, string accessToken);
    }
}