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

    }
}