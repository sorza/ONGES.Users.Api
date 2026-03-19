using System;
using System.Collections.Generic;
using System.Text;

namespace ONGES.Users.Infrastructure.Users.Requests
{
    public sealed record AuthRequest(string Email, string Password);
}
