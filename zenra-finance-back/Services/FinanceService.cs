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
                    .OrderByDescending(f => f.Id)
                    .ToListAsync();
                return Response<List<Finance>>.Success(finances, "Finances retrieved successfully");
            }
            catch (Exception ex)
            {
                return Response<List<Finance>>.Failure("Failed to retrieve finances", ex.ToString());
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