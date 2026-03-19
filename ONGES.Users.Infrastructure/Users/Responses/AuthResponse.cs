namespace ONGES.Users.Infrastructure.Users.Responses
{
    public sealed record AuthResponse(string AccessToken, DateTime ExpiresAt);
}
