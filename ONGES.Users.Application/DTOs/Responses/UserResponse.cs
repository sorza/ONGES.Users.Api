using ONGES.Users.Domain.Users.Enums;
using ONGES.Users.Domain.Users.ValueObjects;

namespace ONGES.Users.Application.DTOs.Responses
{
    public sealed record UserResponse(Guid Id, string Name, Email Email, string ProfileType, bool Active);
}
