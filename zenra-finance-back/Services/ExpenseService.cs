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
    public class ExpenseService : IExpenseService
    {
        private readonly AppDbContext _context;

        public ExpenseService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Response<Expense>> AddExpense(Expense expense, string accessToken)
        {
            TokenService tokenService = new TokenService();
            var userID = await tokenService.ValidateToken(accessToken);
            expense.UserId = userID;
            try
            {
                await _context.Expenses.AddAsync(expense);
                await _context.SaveChangesAsync();
                return Response<Expense>.Success(null, "Expense created successfully");
            }
            catch (Exception ex)
            {
                return Response<Expense>.Failure("Failed to create finance", ex.ToString());
            }
        }

        public async Task<Response<List<Expense>>> GetAllExpense()
        {
            try
            {
                var expenses = await _context.Expenses
                    .OrderByDescending(f => f.Date)
                    .ToListAsync();
                return Response<List<Expense>>.Success(expenses, "Expenses retrieved successfully");
            }
            catch (Exception ex)
            {
                return Response<List<Expense>>.Failure("Failed to retrieve finances", ex.ToString());
            }
        }

        public async Task<Response<List<Expense>>> GetExpense(string accessToken)
        {
            TokenService tokenService = new TokenService();
            var userID = await tokenService.ValidateToken(accessToken);
            try
            {
                var expenses = await _context.Expenses
                .Where(f => f.UserId == userID)
                    .OrderByDescending(f => f.Date)
                    .ToListAsync();
                return Response<List<Expense>>.Success(expenses, "Expenses retrieved successfully");
            }
            catch (Exception ex)
            {
                return Response<List<Expense>>.Failure("Failed to retrieve finances", ex.ToString());
            }
        }

        public async Task<Response<MonthExpenseResponse>> GetThisMonthlyExpensesCount(string accessToken)
        {
            TokenService tokenService = new TokenService();
            var userID = await tokenService.ValidateToken(accessToken);
            try
            {
                var currentMonth = DateTime.UtcNow;
                var monthlyFinances = await _context.Expenses
                    .Where(f => f.Date.Year == currentMonth.Year && f.Date.Month == currentMonth.Month && f.UserId == userID)
                    .ToListAsync();

                var totalAmount = monthlyFinances.Sum(f => f.Amount);
                var monthName = currentMonth.ToString("MMMM");

                return Response<MonthExpenseResponse>.Success(new MonthExpenseResponse
                {
                    Month = monthName,
                    Amount = totalAmount
                }, "Monthly expenses count retrieved successfully");
            }
            catch (Exception ex)
            {
                return Response<MonthExpenseResponse>.Failure("Failed to retrieve monthly finance count", ex.ToString());
            }
        }

        public async Task<Response<decimal>> GetTodayExpensesCount(string accessToken)
        {
            TokenService tokenService = new TokenService();
            var userID = await tokenService.ValidateToken(accessToken);
            try
            {
                var today = DateOnly.FromDateTime(DateTime.UtcNow);
                var dailyExpenses = await _context.Expenses
                    .Where(f => f.Date == today && f.UserId == userID)
                    .ToListAsync();

                var totalAmount = dailyExpenses.Sum(f => f.Amount);

                return Response<decimal>.Success(totalAmount, "Today's expenses count retrieved successfully");
            }
            catch (Exception ex)
            {
                return Response<decimal>.Failure("Failed to retrieve today's expenses count", ex.ToString());
            }
        }

        public async Task<Response<decimal>> GetAllExpensesCount(string accessToken)
        {
            TokenService tokenService = new TokenService();
            var userID = await tokenService.ValidateToken(accessToken);
            try
            {
                var allExpenses = await _context.Expenses
                    .Where(f => f.UserId == userID)
                    .ToListAsync();

                var totalAmount = allExpenses.Sum(f => f.Amount);

                return Response<decimal>.Success(totalAmount, "All expenses count retrieved successfully");
            }
            catch (Exception ex)
            {
                return Response<decimal>.Failure("Failed to retrieve all expenses count", ex.ToString());
            }
        }

        public async Task<Response<List<CurrentWeekDailyExpenseResponse>>> GetCurrentWeekDailyExpenseCount(string accessToken)
        {
            TokenService tokenService = new TokenService();
            var userID = await tokenService.ValidateToken(accessToken);
            try
            {
                var today = DateOnly.FromDateTime(DateTime.UtcNow);
                var daysSinceSunday = (int)today.DayOfWeek;
                var weekStart = today.AddDays(-daysSinceSunday);

                var weekEnd = weekStart.AddDays(6);
                var dailyExpenses = await _context.Expenses
                    .Where(f => f.Date >= weekStart && f.Date <= weekEnd && f.UserId == userID)
                    .GroupBy(f => f.Date)
                    .Select(g => new CurrentWeekDailyExpenseResponse
                    {
                        Day = g.Key.ToString("ddd"),
                        Amount = g.Sum(f => f.Amount)
                    })
                    .ToListAsync();

                var result = new List<CurrentWeekDailyExpenseResponse>();
                for (int i = 0; i < 7; i++)
                {
                    var currentDay = weekStart.AddDays(i);
                    var dayName = currentDay.ToString("ddd");
                    var existingRecord = dailyExpenses.FirstOrDefault(d => d.Day == dayName);

                    result.Add(new CurrentWeekDailyExpenseResponse
                    {
                        Day = dayName,
                        Amount = existingRecord?.Amount ?? 0m
                    });
                }

                return Response<List<CurrentWeekDailyExpenseResponse>>.Success(
                    result,
                    "Current week daily expenses counts retrieved successfully"
                );
            }
            catch (Exception ex)
            {
                return Response<List<CurrentWeekDailyExpenseResponse>>.Failure(
                    "Failed to retrieve current week daily expenses counts",
                    ex.ToString()
                );
            }
        }

        public async Task<Response<List<MonthExpenseResponse>>> GetExpenseeByYear(int year, string accessToken)
        {
            TokenService tokenService = new TokenService();
            var userID = await tokenService.ValidateToken(accessToken);
            try
            {
                var allMonths = Enumerable.Range(1, 12)
                    .Select(m => new MonthExpenseResponse
                    {
                        Month = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(m),
                        Amount = 0
                    });

                var monthlyExpenses = await _context.Expenses
                    .Where(f => f.Date.Year == year && f.UserId == userID)
                    .GroupBy(f => f.Date.Month)
                    .Select(g => new MonthExpenseResponse
                    {
                        Month = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(g.Key),
                        Amount = g.Sum(f => f.Amount)
                    })
                    .ToListAsync();

                var result = allMonths
                    .GroupJoin(monthlyExpenses,
                    all => all.Month,
                    actual => actual.Month,
                    (all, actual) => actual.Any()
                        ? actual.First()
                        : all)
                    .ToList();

                return Response<List<MonthExpenseResponse>>.Success(result, "Monthly finance count retrieved successfully");
            }
            catch (Exception ex)
            {
                return Response<List<MonthExpenseResponse>>.Failure("Failed to retrieve monthly finance count", ex.ToString());
            }
        }
        public async Task<Response<List<CurrentWeekDailyIncomeExpenseResponse>>> GetCurrentWeekDailyIncomeExpenseCount(string accessToken)
        {
            TokenService tokenService = new TokenService();
            var userID = await tokenService.ValidateToken(accessToken);
            try
            {
                var today = DateOnly.FromDateTime(DateTime.UtcNow);
                var daysSinceSunday = (int)today.DayOfWeek;
                var weekStart = today.AddDays(-daysSinceSunday);
                var weekEnd = weekStart.AddDays(6);

                // Get expenses
                var dailyExpenses = await _context.Expenses
                    .Where(f => f.Date >= weekStart && f.Date <= weekEnd && f.UserId == userID)
                    .GroupBy(f => f.Date)
                    .Select(g => new
                    {
                        Day = g.Key.ToString("ddd"),
                        Amount = g.Sum(f => f.Amount)
                    })
                    .ToListAsync();

                // Get income (finances)
                var dailyFinances = await _context.Finances
                    .Where(f => f.Date >= weekStart && f.Date <= weekEnd && f.UserId == userID)
                    .GroupBy(f => f.Date)
                    .Select(g => new
                    {
                        Day = g.Key.ToString("ddd"),
                        Amount = g.Sum(f => f.Amount)
                    })
                    .ToListAsync();

                // Combine results
                var result = new List<CurrentWeekDailyIncomeExpenseResponse>();
                for (int i = 0; i < 7; i++)
                {
                    var currentDay = weekStart.AddDays(i);
                    var dayName = currentDay.ToString("ddd");
                    var expenseRecord = dailyExpenses.FirstOrDefault(d => d.Day == dayName);
                    var incomeRecord = dailyFinances.FirstOrDefault(d => d.Day == dayName);

                    result.Add(new CurrentWeekDailyIncomeExpenseResponse
                    {
                        Day = dayName,
                        AmountExpense = expenseRecord?.Amount ?? 0m,
                        AmountIncome = incomeRecord?.Amount ?? 0m
                    });
                }

                return Response<List<CurrentWeekDailyIncomeExpenseResponse>>.Success(
                    result,
                    "Current week daily income and expense counts retrieved successfully"
                );
            }
            catch (Exception ex)
            {
                return Response<List<CurrentWeekDailyIncomeExpenseResponse>>.Failure(
                    "Failed to retrieve current week daily income and expense counts",
                    ex.ToString()
                );
            }
        }

        public async Task<Response<List<MonthIncomeExpenseResponse>>> GetIncomeExpenseeByYear(int year, string accessToken)
        {
            TokenService tokenService = new TokenService();
            var userID = await tokenService.ValidateToken(accessToken);
            try
            {
                var allMonths = Enumerable.Range(1, 12)
                    .Select(m => new MonthIncomeExpenseResponse
                    {
                        Month = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(m),
                        AmountIncome = 0,
                        AmountExpense = 0
                    });

                var monthlyExpenses = await _context.Expenses
                    .Where(f => f.Date.Year == year && f.UserId == userID)
                    .GroupBy(f => f.Date.Month)
                    .Select(g => new MonthIncomeExpenseResponse
                    {
                        Month = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(g.Key),
                        AmountExpense = g.Sum(f => f.Amount),
                        AmountIncome = 0
                    })
                    .ToListAsync();

                var monthlyFinances = await _context.Finances
                    .Where(f => f.Date.Year == year && f.UserId == userID)
                    .GroupBy(f => f.Date.Month)
                    .Select(g => new MonthIncomeExpenseResponse
                    {
                        Month = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(g.Key),
                        AmountIncome = g.Sum(f => f.Amount),
                        AmountExpense = 0
                    })
                    .ToListAsync();

                var result = allMonths
                    .GroupJoin(monthlyExpenses,
                    all => all.Month,
                    actual => actual.Month,
                    (all, actual) => actual.Any()
                        ? actual.First()
                        : all)
                    .GroupJoin(monthlyFinances,
                    all => all.Month,
                    actual => actual.Month,
                    (all, actual) =>
                        new MonthIncomeExpenseResponse
                        {
                            Month = all.Month,
                            AmountExpense = all.AmountExpense + (actual.Any() ? actual.First().AmountExpense : 0),
                            AmountIncome = all.AmountIncome + (actual.Any() ? actual.First().AmountIncome : 0)
                        })
                    .ToList();

                return Response<List<MonthIncomeExpenseResponse>>.Success(result, "Monthly finance count retrieved successfully");
            }
            catch (Exception ex)
            {
                return Response<List<MonthIncomeExpenseResponse>>.Failure("Failed to retrieve monthly finance count", ex.ToString());
            }
        }

        public async Task<Response<CurrentWeekExpenseResponse>> GetCurrentWeekExpense(string accessToken)
        {
            try
            {
                TokenService tokenService = new TokenService();
                var userId = await tokenService.ValidateToken(accessToken);

                if (userId <= 0)
                {
                    return Response<CurrentWeekExpenseResponse>.Failure("Invalid user token");
                }

                var today = DateOnly.FromDateTime(DateTime.UtcNow);
                var daysSinceSunday = (int)today.DayOfWeek;
                var weekStart = today.AddDays(-daysSinceSunday);
                var weekEnd = weekStart.AddDays(6);

                var totalExpense = await _context.Expenses
                    .Where(e =>
                        e.UserId == userId &&
                        e.Date >= weekStart &&
                        e.Date <= weekEnd
                    )
                    .SumAsync(e => (decimal?)e.Amount) ?? 0m;

                return Response<CurrentWeekExpenseResponse>.Success(
                    new CurrentWeekExpenseResponse
                    {
                        Amount = totalExpense
                    },
                    "Current week expense total retrieved successfully"
                );
            }
            catch (Exception ex)
            {
                return Response<CurrentWeekExpenseResponse>.Failure(
                    "Failed to retrieve current week expense total",
                    ex.ToString()
                );
            }
        }

        public async Task<Response<List<CurrentMonthWeeklyIncomeExpenseResponse>>>
            GetCurrentMonthWeeklyIncomeExpense(string accessToken)
        {
            try
            {
                TokenService tokenService = new TokenService();
                var userId = await tokenService.ValidateToken(accessToken);

                var now = DateTime.UtcNow;
                var monthStart = new DateOnly(now.Year, now.Month, 1);
                var monthEnd = monthStart.AddMonths(1).AddDays(-1);

                var weeklyExpenses = await _context.Expenses
                    .Where(e =>
                        e.UserId == userId &&
                        e.Date >= monthStart &&
                        e.Date <= monthEnd
                    )
                    .GroupBy(e => ((e.Date.Day - 1) / 7) + 1)
                    .Select(g => new
                    {
                        Week = g.Key,
                        Amount = g.Sum(x => x.Amount)
                    })
                    .ToListAsync();

                var weeklyIncome = await _context.Finances
                    .Where(f =>
                        f.UserId == userId &&
                        f.Date >= monthStart &&
                        f.Date <= monthEnd
                    )
                    .GroupBy(f => ((f.Date.Day - 1) / 7) + 1)
                    .Select(g => new
                    {
                        Week = g.Key,
                        Amount = g.Sum(x => x.Amount)
                    })
                    .ToListAsync();

                var result = Enumerable.Range(1, 5)
                    .Select(week =>
                    {
                        var expense = weeklyExpenses.FirstOrDefault(x => x.Week == week);
                        var income = weeklyIncome.FirstOrDefault(x => x.Week == week);

                        return new CurrentMonthWeeklyIncomeExpenseResponse
                        {
                            Week = $"Week {week}",
                            AmountExpense = expense?.Amount ?? 0m,
                            AmountIncome = income?.Amount ?? 0m
                        };
                    })
                    .ToList();

                return Response<List<CurrentMonthWeeklyIncomeExpenseResponse>>.Success(
                    result,
                    "Current month weekly income and expense retrieved successfully"
                );
            }
            catch (Exception ex)
            {
                return Response<List<CurrentMonthWeeklyIncomeExpenseResponse>>.Failure(
                    "Failed to retrieve current month weekly income and expense",
                    ex.ToString()
                );
            }
        }



    }

}