using ONGES.Users.Application.Shared.Results;
using ONGES.Users.Infrastructure.Users.Requests;
using ONGES.Users.Infrastructure.Users.Responses;
using System.Linq.Expressions;

namespace ONGES.Users.Application.Users.Services
{
    public interface IUserService
    {
        Task<Result<UserResponse>> AddUserAsync(UserRequest request, string correlationId, CancellationToken cancellationToken = default);
        Task<Result<AuthResponse>> AuthAsync(AuthRequest request, string ip, string device, string correlationId, CancellationToken cancellationToken = default);
        Task<Result<UserResponse>> GetUserAsync(Expression<Func<UserResponse, bool>> predicate, CancellationToken cancellationToken = default);
        Task<Result<IEnumerable<UserResponse>>> GetAllUsersAsync(Expression<Func<UserResponse, bool>> predicate, CancellationToken cancellationToken = default);
        Task<Result> RemoveUserAsync(Guid id, string correlationId, CancellationToken cancellationToken = default);
    }
}
