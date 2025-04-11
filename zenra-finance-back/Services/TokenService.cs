using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using zenra_finance_back.Data;
using zenra_finance_back.Services.IServices;

namespace zenra_finance_back.Services
{
    public class TokenService
    {
        private readonly AppDbContext _context;
        public async Task<string> GenerateToken(User user)
        {
            return await Task.FromResult(GenerateJwtToken(user));
        }

        public async Task<int> ValidateToken(string token)
        {
            return await Task.FromResult(GetUserByToken(token));
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email)
            };

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

        private int GetUserByToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim != null)
            {
                return int.Parse(userIdClaim.Value);
            }
            throw new Exception("Invalid token");
        }

    }
}