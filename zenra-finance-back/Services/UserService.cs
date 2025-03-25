using System;
using System.Threading.Tasks;
using zenra_finance_back.Data;
using zenra_finance_back.Models;
using zenra_finance_back.Services.IServices;

namespace zenra_finance_back.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Response<User>> Register(User user)
        {
            user.CreatedAt = DateTime.Now; // Use of DateTime.UtcNow is recommended for consistency across time zones
            try
            {
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
                return Response<User>.Success(user, "Registered Successfully");
            }
            catch (Exception ex)
            {
                // Log the exception details here
                return Response<User>.Failure("Registration failed. Please try again.", ex.ToString());
            }
        }
    }
}
