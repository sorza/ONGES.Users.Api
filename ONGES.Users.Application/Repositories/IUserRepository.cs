using ONGES.Users.Domain.Users.Entities;
using ONGES.Users.Domain.Users.ValueObjects;

namespace ONGES.Users.Application.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> Auth(Email email, CancellationToken cancellationToken);
    }
}
