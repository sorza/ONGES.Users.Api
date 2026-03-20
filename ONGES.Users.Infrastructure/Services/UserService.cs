using FluentValidation;
using ONGES.Users.Application.DTOs.Requests;
using ONGES.Users.Application.DTOs.Responses;
using ONGES.Users.Application.Repositories;
using ONGES.Users.Application.Services;
using ONGES.Users.Domain.Shared.Results;
using ONGES.Users.Domain.Users.Entities;
using ONGES.Users.Domain.Users.Enums;
using ONGES.Users.Domain.Users.ValueObjects;
using System.Linq.Expressions;

namespace ONGES.Users.Infrastructure.Services
{
    public class UserService(IUserRepository repository,
                             IValidator<UserRequest> userValidator,
                             IValidator<AuthRequest> authValidator) : IUserService
    {
        public async Task<Result<UserResponse>> AddUserAsync(UserRequest request, string correlationId, CancellationToken cancellationToken = default)
        {
            var validation = userValidator.Validate(request);

            if(!validation.IsValid)
                return Result.Failure<UserResponse>(new Error("400", string.Join(", ", validation.Errors.Select(e => e.ErrorMessage))));

            var userExists = await repository.ExistsAsync(u => u.Email == request.Email, cancellationToken);

            if(userExists)
                return Result.Failure<UserResponse>(new Error("409", "Este usuário já está cadastrado."));

            var user = User.Create(request.Name, Email.Create(request.Email), request.Password, EProfileType.Doador);

            //TODO: Criar evento de usuário criado
            //TODO: Anexar evento ao eventStore
            //TODO: Publicar evento para fila de mensagens

            await repository.CreateAsync(user, cancellationToken);

            return Result.Success(new UserResponse(user.Id, user.Name, user.Email, user.Profile.ToString(), user.Active));

        }

        public Task<Result<AuthResponse>> AuthAsync(AuthRequest request, string ip, string device, string correlationId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<Result<IEnumerable<UserResponse>>> GetAllUsersAsync(Expression<Func<User, bool>> predicate, CancellationToken cancellationToken = default)
        {
            var result = await repository.GetAllAsync(predicate, cancellationToken);

            var userResponses = result!.Select(user => new UserResponse(user.Id, user.Name, user.Email, user.Profile.ToString(), user.Active));
            return Result.Success(userResponses);
        }

        public async Task<Result<UserResponse>> GetUserAsync(Expression<Func<User, bool>> predicate, CancellationToken cancellationToken = default)
        {
            var user = await repository.GetAsync(predicate, cancellationToken);

            if(user is null)
                return Result.Failure<UserResponse>(new Error("404", "Usuário não encontrado."));

            return Result.Success(new UserResponse(user.Id, user.Name, user.Email, user.Profile.ToString(), user.Active));
        }

        public async Task<Result> RemoveUserAsync(Guid id, string correlationId, CancellationToken cancellationToken = default)
        {
            var user = await repository.GetAsync(u => u.Id == id, cancellationToken);

            if(user is null)
                return Result.Failure(new Error("404", "Usuário não encontrado."));

            //TODO: Criar evento de usuário removido
            //TODO: Anexar evento ao eventStore
            //TODO: Publicar evento para fila de mensagens

            await repository.DeleteAsync(user.Id, cancellationToken);
            return Result.Success(new UserResponse(user.Id, user.Name, user.Email, user.Profile.ToString(), user.Active));
        }
    }
}
