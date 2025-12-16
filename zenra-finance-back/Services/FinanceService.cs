using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using zenra_finance_back.Data;
using zenra_finance_back.Models;
using zenra_finance_back.Services.IServices;

namespace zenra_finance_back.Services
{
    public class FinanceService : IFinanceService
    {
        private readonly AppDbContext _context;

        public FinanceService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Response<Finance>> AddFinance(Finance finance, string accessToken)
        {
            TokenService tokenService = new TokenService();
            var userID = await tokenService.ValidateToken(accessToken);
            finance.UserId = userID;
            try
            {
                await _context.Finances.AddAsync(finance);
                await _context.SaveChangesAsync();
                return Response<Finance>.Success(null, "Finance created successfully");
            }
            catch (Exception ex)
            {
                return Response<Finance>.Failure("Failed to create finance", ex.ToString());
            }
        }

        public async Task<Response<List<Finance>>> GetAllFinance()
        {
            try
            {
                var finances = await _context.Finances
                    .OrderByDescending(f => f.Date)
                    .ToListAsync();
                return Response<List<Finance>>.Success(finances, "Finances retrieved successfully");
            }
            catch (Exception ex)
            {
                return Response<List<Finance>>.Failure("Failed to retrieve finances", ex.ToString());
            }
        }

        public async Task<Response<List<Finance>>> GetFinance(string accessToken)
        {
            TokenService tokenService = new TokenService();
            var userID = await tokenService.ValidateToken(accessToken);
            try
            {
                var finances = await _context.Finances
                    .OrderByDescending(f => f.Date)
                    .Where(f => f.UserId == userID)
                    .ToListAsync();
                return Response<List<Finance>>.Success(finances, "Finances retrieved successfully");
            }
            catch (Exception ex)
            {
                return Response<List<Finance>>.Failure("Failed to retrieve finances", ex.ToString());
            }
        }

        public async Task<Response<MonthFinanceResponse>> GetThisMonthlyFinanceCount(string accessToken)
        {
            TokenService tokenService = new TokenService();
            var userID = await tokenService.ValidateToken(accessToken);
            try
            {
                var currentMonth = DateTime.UtcNow;
                var monthlyFinances = await _context.Finances
                    .Where(f => f.Date.Year == currentMonth.Year && f.Date.Month == currentMonth.Month && f.UserId == userID)
                    .ToListAsync();

                var totalAmount = monthlyFinances.Sum(f => f.Amount);
                var monthName = currentMonth.ToString("MMMM");

                return Response<MonthFinanceResponse>.Success(new MonthFinanceResponse
                {
                    Month = monthName,
                    Amount = totalAmount
                }, "Monthly finance count retrieved successfully");
            }
            catch (Exception ex)
            {
                return Response<MonthFinanceResponse>.Failure("Failed to retrieve monthly finance count", ex.ToString());
            }
        }

        public async Task<Response<MonthFinanceResponse>> GetLastMonthlyFinanceCount(string accessToken)
        {
            TokenService tokenService = new TokenService();
            var userID = await tokenService.ValidateToken(accessToken);
            try
            {
                var lastMonth = DateTime.UtcNow.AddMonths(-1);
                var monthlyFinances = await _context.Finances
                    .Where(f => f.Date.Year == lastMonth.Year && f.Date.Month == lastMonth.Month && f.UserId == userID)
                    .ToListAsync();

                var totalAmount = monthlyFinances.Sum(f => f.Amount);
                var monthName = lastMonth.ToString("MMMM");

                return Response<MonthFinanceResponse>.Success(new MonthFinanceResponse
                {
                    Month = monthName,
                    Amount = totalAmount
                }, "Monthly finance count retrieved successfully");
            }
            catch (Exception ex)
            {
                return Response<MonthFinanceResponse>.Failure("Failed to retrieve monthly finance count", ex.ToString());
            }
        }

        public async Task<Response<YearFinanceResponse>> GetThisYearFinanceCount(string accessToken)
        {
            TokenService tokenService = new TokenService();
            var userID = await tokenService.ValidateToken(accessToken);
            try
            {
                var currentYear = DateTime.UtcNow.Year;
                var yearlyFinances = await _context.Finances
                    .Where(f => f.Date.Year == currentYear && f.UserId == userID)
                    .ToListAsync();

                var totalAmount = yearlyFinances.Sum(f => f.Amount);

                return Response<YearFinanceResponse>.Success(new YearFinanceResponse
                {
                    Year = currentYear.ToString(),
                    Amount = totalAmount
                }, "Yearly finance count retrieved successfully");
            }
            catch (Exception ex)
            {
                return Response<YearFinanceResponse>.Failure("Failed to retrieve yearly finance count", ex.ToString());
            }
        }

        public async Task<Response<YearFinanceResponse>> GetLastYearFinanceCount(string accessToken)
        {
            TokenService tokenService = new TokenService();
            var userID = await tokenService.ValidateToken(accessToken);
            try
            {
                var lastYear = DateTime.UtcNow.AddYears(-1).Year;
                var yearlyFinances = await _context.Finances
                    .Where(f => f.Date.Year == lastYear && f.UserId == userID)
                    .ToListAsync();

                var totalAmount = yearlyFinances.Sum(f => f.Amount);

                return Response<YearFinanceResponse>.Success(new YearFinanceResponse
                {
                    Year = lastYear.ToString(),
                    Amount = totalAmount
                }, "Yearly finance count retrieved successfully");
            }
            catch (Exception ex)
            {
                return Response<YearFinanceResponse>.Failure("Failed to retrieve yearly finance count", ex.ToString());
            }
        }

        public async Task<Response<List<MonthFinanceResponse>>> GetFinanceByYear(int year, string accessToken)
        {
            TokenService tokenService = new TokenService();
            var userID = await tokenService.ValidateToken(accessToken);
            try
            {
                var allMonths = Enumerable.Range(1, 12)
                    .Select(m => new MonthFinanceResponse
                    {
                        Month = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(m),
                        Amount = 0
                    });

                var monthlyFinances = await _context.Finances
                    .Where(f => f.Date.Year == year && f.UserId == userID)
                    .GroupBy(f => f.Date.Month)
                    .Select(g => new MonthFinanceResponse
                    {
                        Month = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(g.Key),
                        Amount = g.Sum(f => f.Amount)
                    })
                    .ToListAsync();

                var result = allMonths
                    .GroupJoin(monthlyFinances,
                    all => all.Month,
                    actual => actual.Month,
                    (all, actual) => actual.Any()
                        ? actual.First()
                        : all)
                    .ToList();

                return Response<List<MonthFinanceResponse>>.Success(result, "Monthly finance count retrieved successfully");
            }
            catch (Exception ex)
            {
                return Response<List<MonthFinanceResponse>>.Failure("Failed to retrieve monthly finance count", ex.ToString());
            }
        }

        public async Task<Response<CurrentWeekFinanceResponse>> GetCurrentWeekDailyFinanceCount(string accessToken)
        {
            try
            {
                TokenService tokenService = new TokenService();
                var userId = await tokenService.ValidateToken(accessToken);

                if (userId <= 0)
                {
                    return Response<CurrentWeekFinanceResponse>.Failure("Invalid user token");
                }

                var today = DateOnly.FromDateTime(DateTime.UtcNow);
                var daysSinceSunday = (int)today.DayOfWeek;
                var weekStart = today.AddDays(-daysSinceSunday);
                var weekEnd = weekStart.AddDays(6);

                var totalAmount = await _context.Finances
                    .Where(f =>
                        f.UserId == userId &&
                        f.Date >= weekStart &&
                        f.Date <= weekEnd
                    )
                    .SumAsync(f => (decimal?)f.Amount) ?? 0m;

                return Response<CurrentWeekFinanceResponse>.Success(
                    new CurrentWeekFinanceResponse
                    {
                        Amount = totalAmount
                    },
                    "Current week finance total retrieved successfully"
                );
            }
            catch (Exception ex)
            {
                return Response<CurrentWeekFinanceResponse>.Failure(
                    "Failed to retrieve current week finance total",
                    ex.ToString()
                );
            }
        }


        public async Task<Response<decimal>> GetAllFinancesCount(string accessToken)
        {
            TokenService tokenService = new TokenService();
            var userID = await tokenService.ValidateToken(accessToken);
            try
            {
                var allFinances = await _context.Finances
                    .Where(f => f.UserId == userID)
                    .ToListAsync();

                var totalAmount = allFinances.Sum(f => f.Amount);

                return Response<decimal>.Success(totalAmount, "All finances count retrieved successfully");
            }
            catch (Exception ex)
            {
                return Response<decimal>.Failure("Failed to retrieve all finances count", ex.ToString());
            }
        }

        public async Task<Response<Finance>> UpdateFinance(int id, Finance finance)
        {
            try
            {
                var existingFinance = await _context.Finances.FindAsync(id);
                if (existingFinance == null)
                {
                    return Response<Finance>.Failure("Finance not found", null);
                }

                existingFinance.Date = finance.Date;
                existingFinance.IncomeType = finance.IncomeType;
                existingFinance.Amount = finance.Amount;

                _context.Finances.Update(existingFinance);
                return Response<Finance>.Success(null, "Finance updated successfully");
            }
            catch (Exception ex)
            {
                return Response<Finance>.Failure("Failed to update finance", ex.ToString());
            }
        }

        public async Task<Response<Finance>> DeleteFinance(int id)
        {
            try
            {
                var finance = await _context.Finances.FindAsync(id);
                if (finance == null)
                {
                    return Response<Finance>.Failure("Finance not found", null);
                }

                _context.Finances.Remove(finance);
                return Response<Finance>.Success(null, "Finance deleted successfully");
            }
            catch (Exception ex)
            {
                return Response<Finance>.Failure("Failed to delete finance", ex.ToString());
            }
        }
    }
}