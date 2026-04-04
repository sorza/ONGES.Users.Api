using ONGES.Users.Application.Events;

namespace ONGES.Users.Application.DTOs.Events
{
    public record UserCreatedEvent(string Name, string Password, string Email, string Profile, bool Active) : IDomainEvent;
   
}
