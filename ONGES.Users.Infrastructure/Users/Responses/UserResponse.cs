using ONGES.Users.Domain.Users.Enums;
using ONGES.Users.Domain.Users.ValueObjects;

namespace ONGES.Users.Infrastructure.Users.Responses
{
    public sealed record UserResponse(Guid Id, string Name, Password Password, Email Email, EProfileType ProfileType, bool Active);
}
