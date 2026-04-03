using ONGES.Users.Domain.Users.Enums;

namespace ONGES.Users.Application.DTOs.Requests
{
    public sealed record UpdateRoleRequest(Guid userId, EProfileType role);
}
