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

    }
}