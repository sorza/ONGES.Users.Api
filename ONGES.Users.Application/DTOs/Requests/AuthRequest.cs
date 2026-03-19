using System;
using System.Collections.Generic;
using System.Text;

namespace ONGES.Users.Application.DTOs.Requests
{
    public sealed record AuthRequest(string Email, string Password);
}
