using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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
            try
            {
                // Hash the password before saving
                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
                return Response<User>.Success(user, "Registered Successfully");
            }
            catch (Exception ex)
            {
                return Response<User>.Failure("Registration failed. Please try again.", ex.ToString());
            }
        }

        public async Task<Response<string>> Login(LoginRequest loginRequest)
        {
            try
            {
                TokenService tokenService = new TokenService();
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == loginRequest.Email);

                if (user == null)
                {
                    return Response<string>.Failure("Invalid credentials.");
                }

                // Verify password
                if (!BCrypt.Net.BCrypt.Verify(loginRequest.Password, user.Password))
                {
                    return Response<string>.Failure("Invalid credentials.");
                }

                // Generate JWT token
                var token = await tokenService.GenerateToken(user);

                return Response<string>.Success(token, "Login successful");
            }
            catch (Exception ex)
            {
                return Response<string>.Failure("Login failed. Please try again.", ex.ToString());
            }
        }
    }

}
