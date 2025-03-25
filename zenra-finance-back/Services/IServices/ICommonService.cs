using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using zenra_finance_back.Models;

namespace zenra_finance_back.Services.IServices
{
    public interface ICommonService
    {
        Task<Response<string>> ConvertImageToBase64Async(IFormFile imageFile);
    }
}
