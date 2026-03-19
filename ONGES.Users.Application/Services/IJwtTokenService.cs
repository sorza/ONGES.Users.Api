using ONGES.Users.Domain.Users.Entities;

namespace ONGES.Users.Application.Services
{
    public interface IJwtTokenService
    {
        TokenInfo CreateToken(User user);
    }
    public record TokenInfo(string Token, DateTime ExpiresAt);
}
