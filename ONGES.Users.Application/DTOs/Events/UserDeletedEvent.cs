using ONGES.Users.Application.Events;
using ONGES.Users.Domain.Users.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ONGES.Users.Application.DTOs.Events
{
    public record UserDeletedEvent(string Email, string Name, EProfileType Profile) : IDomainEvent;
    
}
