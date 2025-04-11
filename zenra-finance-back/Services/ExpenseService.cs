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

        public async Task<Response<Expense>> AddExpense(Expense expense)
        {
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

        public async Task<Response<List<Expense>>> GetExpense()
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

        public async Task<Response<MonthExpenseResponse>> GetThisMonthlyExpensesCount()
        {
            try
            {
                var currentMonth = DateTime.UtcNow;
                var monthlyFinances = await _context.Expenses
                    .Where(f => f.Date.Year == currentMonth.Year && f.Date.Month == currentMonth.Month)
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

        public async Task<Response<decimal>> GetTodayExpensesCount()
        {
            try
            {
                var today = DateOnly.FromDateTime(DateTime.UtcNow);
                var dailyExpenses = await _context.Expenses
                    .Where(f => f.Date == today)
                    .ToListAsync();

                var totalAmount = dailyExpenses.Sum(f => f.Amount);

                return Response<decimal>.Success(totalAmount, "Today's expenses count retrieved successfully");
            }
            catch (Exception ex)
            {
                return Response<decimal>.Failure("Failed to retrieve today's expenses count", ex.ToString());
            }
        }

        public async Task<Response<decimal>> GetAllExpensesCount()
        {
            try
            {
                var allExpenses = await _context.Expenses
                    .ToListAsync();

                var totalAmount = allExpenses.Sum(f => f.Amount);

                return Response<decimal>.Success(totalAmount, "All expenses count retrieved successfully");
            }
            catch (Exception ex)
            {
                return Response<decimal>.Failure("Failed to retrieve all expenses count", ex.ToString());
            }
        }

        public async Task<Response<List<CurrentWeekDailyExpenseResponse>>> GetCurrentWeekDailyExpenseCount()
        {
            try
            {
                var today = DateOnly.FromDateTime(DateTime.UtcNow);
                var daysSinceSunday = (int)today.DayOfWeek;
                var weekStart = today.AddDays(-daysSinceSunday);

                var weekEnd = weekStart.AddDays(6);
                var dailyExpenses = await _context.Expenses
                    .Where(f => f.Date >= weekStart && f.Date <= weekEnd)
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

        public async Task<Response<List<MonthExpenseResponse>>> GetExpenseeByYear(int year)
        {
            try
            {
                var allMonths = Enumerable.Range(1, 12)
                    .Select(m => new MonthExpenseResponse
                    {
                        Month = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(m),
                        Amount = 0
                    });

                var monthlyExpenses = await _context.Expenses
                    .Where(f => f.Date.Year == year)
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
        public async Task<Response<List<CurrentWeekDailyIncomeExpenseResponse>>> GetCurrentWeekDailyIncomeExpenseCount()
        {
            try
            {
                var today = DateOnly.FromDateTime(DateTime.UtcNow);
                var daysSinceSunday = (int)today.DayOfWeek;
                var weekStart = today.AddDays(-daysSinceSunday);
                var weekEnd = weekStart.AddDays(6);

                // Get expenses
                var dailyExpenses = await _context.Expenses
                    .Where(f => f.Date >= weekStart && f.Date <= weekEnd)
                    .GroupBy(f => f.Date)
                    .Select(g => new
                    {
                        Day = g.Key.ToString("ddd"),
                        Amount = g.Sum(f => f.Amount)
                    })
                    .ToListAsync();

                // Get income (finances)
                var dailyFinances = await _context.Finances
                    .Where(f => f.Date >= weekStart && f.Date <= weekEnd)
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

        public async Task<Response<List<MonthIncomeExpenseResponse>>> GetMonthlyIncomeExpenseCount()
        {
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
                    .GroupBy(f => f.Date.Month)
                    .Select(g => new MonthIncomeExpenseResponse
                    {
                        Month = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(g.Key),
                        AmountExpense = g.Sum(f => f.Amount),
                        AmountIncome = 0
                    })
                    .ToListAsync();

                var monthlyFinances = await _context.Finances
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

                return Response<List<MonthIncomeExpenseResponse>>.Success(result, "Monthly income and expense count retrieved successfully");
            }
            catch (Exception ex)
            {
                return Response<List<MonthIncomeExpenseResponse>>.Failure("Failed to retrieve monthly income and expense count", ex.ToString());
            }
        }

    }

}