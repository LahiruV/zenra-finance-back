using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using zenra_finance_back.Models;

namespace zenra_finance_back.Services.IServices
{
    public interface IFinanceService
    {
        Task<Response<Finance>> AddFinance(Finance finance);
        Task<Response<List<Finance>>> GetFinance();
        Task<Response<Finance>> UpdateFinance(int id, Finance finance);
        Task<Response<Finance>> DeleteFinance(int id);
    }
}