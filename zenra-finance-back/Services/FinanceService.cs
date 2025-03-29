using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
                return Response<Finance>.Success(null, "Finance created successfully");
            }
            catch (Exception ex)
            {
                return Response<Finance>.Failure("Failed to create finance", ex.ToString());
            }
        }
    }
}