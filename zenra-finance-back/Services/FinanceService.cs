using System;
using System.Collections.Generic;
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

        public async Task<Response<Finance>> AddFinance(Finance finance)
        {
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

        public async Task<Response<List<Finance>>> GetFinance()
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

        public async Task<Response<MonthFinanceResponse>> GetThisMonthlyFinanceCount()
        {
            try
            {
                var currentMonth = DateTime.UtcNow;
                var monthlyFinances = await _context.Finances
                    .Where(f => f.Date.Year == currentMonth.Year && f.Date.Month == currentMonth.Month)
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

        public async Task<Response<MonthFinanceResponse>> GetLastMonthlyFinanceCount()
        {
            try
            {
                var lastMonth = DateTime.UtcNow.AddMonths(-1);
                var monthlyFinances = await _context.Finances
                    .Where(f => f.Date.Year == lastMonth.Year && f.Date.Month == lastMonth.Month)
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

        public async Task<Response<YearFinanceResponse>> GetThisYearFinanceCount()
        {
            try
            {
                var currentYear = DateTime.UtcNow.Year;
                var yearlyFinances = await _context.Finances
                    .Where(f => f.Date.Year == currentYear)
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

        public async Task<Response<YearFinanceResponse>> GetLastYearFinanceCount()
        {
            try
            {
                var lastYear = DateTime.UtcNow.AddYears(-1).Year;
                var yearlyFinances = await _context.Finances
                    .Where(f => f.Date.Year == lastYear)
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