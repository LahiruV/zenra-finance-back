using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using zenra_finance_back.Models;

namespace zenra_finance_back.Services.IServices
{
    public interface IFinanceService
    {
        Task<Response<Finance>> AddFinance(Finance finance, string accessToken);
        Task<Response<List<Finance>>> GetFinance();
        Task<Response<MonthFinanceResponse>> GetThisMonthlyFinanceCount();
        Task<Response<MonthFinanceResponse>> GetLastMonthlyFinanceCount();
        Task<Response<YearFinanceResponse>> GetThisYearFinanceCount();
        Task<Response<YearFinanceResponse>> GetLastYearFinanceCount();
        Task<Response<List<MonthFinanceResponse>>> GetFinanceByYear(int year);
        Task<Response<List<CurrentWeekDailyFinanceResponse>>> GetCurrentWeekDailyFinanceCount();
        Task<Response<decimal>> GetAllFinancesCount();
        Task<Response<Finance>> UpdateFinance(int id, Finance finance);
        Task<Response<Finance>> DeleteFinance(int id);
    }
}