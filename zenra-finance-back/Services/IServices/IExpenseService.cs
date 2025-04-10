using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using zenra_finance_back.Models;

namespace zenra_finance_back.Services.IServices
{
    public interface IExpenseService
    {
        Task<Response<Expense>> AddExpense(Expense expense);
        Task<Response<List<Expense>>> GetExpense();
        Task<Response<MonthExpenseResponse>> GetThisMonthlyExpensesCount();
        Task<Response<decimal>> GetTodayExpensesCount();
        Task<Response<decimal>> GetAllExpensesCount();
        Task<Response<List<CurrentWeekDailyExpenseResponse>>> GetCurrentWeekDailyExpenseCount();
        Task<Response<List<MonthExpenseResponse>>> GetExpenseeByYear(int year);
        Task<Response<List<CurrentWeekDailyIncomeExpenseResponse>>> GetCurrentWeekDailyIncomeExpenseCount();
    }
}