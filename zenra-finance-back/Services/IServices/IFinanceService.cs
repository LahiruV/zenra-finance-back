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
        Task<Response<List<Finance>>> GetFinance(string accessToken);
        Task<Response<MonthFinanceResponse>> GetThisMonthlyFinanceCount(string accessToken);
        Task<Response<MonthFinanceResponse>> GetLastMonthlyFinanceCount(string accessToken);
        Task<Response<YearFinanceResponse>> GetThisYearFinanceCount(string accessToken);
        Task<Response<YearFinanceResponse>> GetLastYearFinanceCount(string accessToken);
        Task<Response<List<MonthFinanceResponse>>> GetFinanceByYear(int year, string accessToken);
        Task<Response<List<CurrentWeekDailyFinanceResponse>>> GetCurrentWeekDailyFinanceCount(string accessToken);
        Task<Response<decimal>> GetAllFinancesCount(string accessToken);
        Task<Response<Finance>> UpdateFinance(int id, Finance finance);
        Task<Response<Finance>> DeleteFinance(int id);
    }
}