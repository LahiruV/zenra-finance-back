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
    }
}