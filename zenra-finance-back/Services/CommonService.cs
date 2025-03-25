using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using zenra_finance_back.Models;
using zenra_finance_back.Services.IServices;

namespace zenra_finance_back.Services
{
    public class CommonService : ICommonService
    {
        public async Task<Response<string>> ConvertImageToBase64Async(IFormFile imageFile)
        {
            try
            {
                using (var ms = new MemoryStream())
                {
                    await imageFile.CopyToAsync(ms);
                    byte[] imageBytes = ms.ToArray();
                    string base64String = Convert.ToBase64String(imageBytes);
                    return Response<string>.Success(base64String, "Image converted successfully.");
                }
            }
            catch (Exception ex)
            {
                return Response<string>.Failure("Failed to convert image to Base64.", ex.Message);
            }
        }
    }
}
