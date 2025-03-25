using System.Threading.Tasks;
using zenra_finance_back.Models;

namespace zenra_finance_back.Services.IServices
{
    public interface IUserService
    {
        Task<Response<User>> Register(User user);
    }
}
