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
                // Log the exception details here
                return Response<User>.Failure("Registration failed. Please try again.", ex.ToString());
            }
        }

        public async Task<Response<string>> Login(LoginRequest loginRequest)
        {
            try
            {
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

                // Generate JWT token (Optional: implement a token service to generate the token)
                var token = GenerateJwtToken(user); 

                return Response<string>.Success(token, "Login successful");
            }
            catch (Exception ex)
            {
                return Response<string>.Failure("Login failed. Please try again.", ex.ToString());
            }
        }

        private string GenerateJwtToken(User user)
        {
            // Example token generation logic
            var claims = new[]
            {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Name, user.Name),
        new Claim(ClaimTypes.Email, user.Email)
    };

            // Use a longer, secure key for encryption (at least 128 bits or 16 characters)
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("YourSuperSecretKeyThatIsAtLeast128BitsLong"));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "zenra_pvt_ltd",
                audience: "zenra_finance",
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
