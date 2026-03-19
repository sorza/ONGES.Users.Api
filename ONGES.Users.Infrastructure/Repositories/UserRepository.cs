using Microsoft.EntityFrameworkCore;
using ONGES.Users.Application.Repositories;
using ONGES.Users.Domain.Users.Entities;
using ONGES.Users.Domain.Users.ValueObjects;
using ONGES.Users.Infrastructure.Data;

namespace ONGES.Users.Infrastructure.Repositories
{
    public class UserRepository(AppDbContext context) : GenericRepository<User>(context), IUserRepository
    {
        protected readonly DbSet<User> _users = context.Set<User>();

        public async Task<User?> Auth(Email email, CancellationToken cancellationToken)
            => await _users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email, cancellationToken);        
    }
}
