using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ONGES.Users.Application.Services;
using ONGES.Users.Domain.Users.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ONGES.Users.Infrastructure.Services
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly IConfiguration _configuration;

        public JwtTokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public TokenInfo CreateToken(User user)
        {
            var key = _configuration["Jwt:Key"];
            if (string.IsNullOrEmpty(key))
                throw new InvalidOperationException("A chave JWT não está configurada.");

            var keyBytes = Convert.FromBase64String(key);
            var signingKey = new SymmetricSecurityKey(keyBytes);
            var signingCredentials = new SigningCredentials(
                signingKey,
                SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("UserId", user.Id.ToString()),
                new Claim("Name", user.Name),
                new Claim(ClaimTypes.Role, user.Profile.ToString())
            };

            var expiresInMinutes = double.Parse(_configuration["Jwt:ExpiresMinutes"]!);
            var expiresAt = DateTime.UtcNow.AddMinutes(expiresInMinutes);

            var jwt = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: expiresAt,
                signingCredentials: signingCredentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(jwt);
            return new TokenInfo(tokenString, expiresAt);
        }
    }
}
