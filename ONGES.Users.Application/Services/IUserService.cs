using ONGES.Users.Application.DTOs.Requests;
using ONGES.Users.Application.DTOs.Responses;
using ONGES.Users.Domain.Shared.Results;
using ONGES.Users.Domain.Users.Entities;
using System.Linq.Expressions;

namespace ONGES.Users.Application.Services
{
    public interface IUserService
    {
        Task<Result<UserResponse>> AddUserAsync(UserRequest request, string correlationId, CancellationToken cancellationToken = default);
        Task<Result<AuthResponse>> AuthAsync(AuthRequest request, string ip, string device, string correlationId, CancellationToken cancellationToken = default);
        Task<Result<UserResponse>> GetUserAsync(Expression<Func<User, bool>> predicate, CancellationToken cancellationToken = default);
        Task<Result<IEnumerable<UserResponse>>> GetAllUsersAsync(CancellationToken cancellationToken = default);
        Task<Result<IEnumerable<UserResponse>>> GetActiveUsersAsync(CancellationToken cancellationToken = default);
        Task<Result> RemoveUserAsync(Guid id, string correlationId, CancellationToken cancellationToken = default);
        Task<Result> DeactivateUserAsync(Guid id, string correlationId, CancellationToken cancellationToken = default);
        Task<Result> ActivateUserAsync(Guid id, string correlationId, CancellationToken cancellationToken = default);
        Task<Result> UpdateRoleUserAsync(UpdateRoleRequest request, string correlationId, CancellationToken cancellationToken = default);
    }
}
