using ONGES.Users.Application.Events;

namespace ONGES.Users.Application.DTOs.Events
{
    public record UserStatusChangedEvent(string Email, bool IsActive) : IDomainEvent;
}
