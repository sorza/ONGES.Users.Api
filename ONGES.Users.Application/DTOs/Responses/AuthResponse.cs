namespace ONGES.Users.Application.DTOs.Responses
{
    public sealed record AuthResponse(string AccessToken, DateTime ExpiresAt);
}
