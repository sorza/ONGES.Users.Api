using ONGES.Users.Application.Events;
using ONGES.Users.Domain.Users.Enums;

namespace ONGES.Users.Application.DTOs.Events
{
    public record UserRoleChangedEvent(string Email, EProfileType Role) : IDomainEvent;    
}
