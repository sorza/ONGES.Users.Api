
using ONGES.Users.Application.Events;

namespace ONGES.Users.Application.DTOs.Events
{
    public record UserLoginEvent(string Name, string Ip, string Device) : IDomainEvent;
}
