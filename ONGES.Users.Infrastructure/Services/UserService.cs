using FluentValidation;
using ONGES.Users.Application.DTOs.Events;
using ONGES.Users.Application.DTOs.Requests;
using ONGES.Users.Application.DTOs.Responses;
using ONGES.Users.Application.Events;
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
                             IJwtTokenService jwtService,
                             IEventStore eventStore,
                             IValidator<UserRequest> userValidator,
                             IValidator<AuthRequest> authValidator) : IUserService
    {
        public async Task<Result<UserResponse>> AddUserAsync(UserRequest request, string correlationId, CancellationToken cancellationToken = default)
        {
            var validation = userValidator.Validate(request);

            if(!validation.IsValid)
                return Result.Failure<UserResponse>(new Error("400", string.Join(", ", validation.Errors.Select(e => e.ErrorMessage))));

            var userExists = await repository.ExistsAsync(u => u.Email.Address == request.Email, cancellationToken);

            if(userExists)
                return Result.Failure<UserResponse>(new Error("409", "Este usuário já está cadastrado."));

            var user = User.Create(request.Name, Email.Create(request.Email), request.Password, EProfileType.Doador);

           
            var evt = new UserCreatedEvent(user.Name, user.Password, user.Email, user.Profile.ToString(), user.Active);
          
            await eventStore.AppendAsync(user.Id.ToString(), evt, 0, correlationId);

            //TODO: Publicar evento para fila de mensagens

            await repository.CreateAsync(user, cancellationToken);

            return Result.Success(new UserResponse(user.Id, user.Name, user.Email, user.Profile.ToString(), user.Active));

        }

        public async Task<Result<AuthResponse>> AuthAsync(AuthRequest request, string ip, string device, string correlationId, CancellationToken cancellationToken = default)
        {

            var validation = authValidator.Validate(request);
            
            if(!validation.IsValid)
                return Result.Failure<AuthResponse>(new Error("400", string.Join(", ", validation.Errors.Select(e => e.ErrorMessage))));

            var email = Email.Create(request.Email);

            var user = await repository.Auth(email, cancellationToken);

            if(user is null)
                return Result.Failure<AuthResponse>(new Error("401", "Credenciais inválidas."));

            if(!user.Password.Verify(request.Password))
                return Result.Failure<AuthResponse>(new Error("401", "Credenciais inválidas."));

            if(!user.Active)
                return Result.Failure<AuthResponse>(new Error("403", "Usuário inativo."));

            var tokenInfo = jwtService.CreateToken(user);
           
            var evt = new UserLoginEvent(user.Name, ip, device);           

            await eventStore.AppendAsync(user.Id.ToString(), evt, 0, correlationId);
           
            //TODO: Publicar evento para fila de mensagens

            return Result.Success(new AuthResponse(tokenInfo.Token, tokenInfo.ExpiresAt));

        }
               
        public async Task<Result<IEnumerable<UserResponse>>> GetAllUsersAsync(CancellationToken cancellationToken = default)
        {
            var result = await repository.GetAllAsync(u => true, cancellationToken);
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

            await repository.DeleteAsync(user.Id, cancellationToken);
            return Result.Success(new UserResponse(user.Id, user.Name, user.Email, user.Profile.ToString(), user.Active));
        }

        public async Task<Result> DeactivateUserAsync(Guid id, string correlationId, CancellationToken cancellationToken = default) 
        {
            var user = await repository.GetAsync(u => u.Id == id, cancellationToken);

            if (user is null)
                return Result.Failure(new Error("404", "Usuário não encontrado."));

            user.Deactivate();

            //TODO: Criar evento de usuário desativado
            //TODO: Anexar evento ao eventStore  

            await repository.UpdateAsync(user, cancellationToken);
            return Result.Success(new UserResponse(user.Id, user.Name, user.Email, user.Profile.ToString(), user.Active));
        }

        public async Task<Result> ActivateUserAsync(Guid id, string correlationId, CancellationToken cancellationToken = default)
        {
            var user = await repository.GetAsync(u => u.Id == id, cancellationToken);
            
            if (user is null)
                return Result.Failure(new Error("404", "Usuário não encontrado."));

            user.Activate();

            //TODO: Criar evento de usuário ativado
            //TODO: Anexar evento ao eventStore  

            await repository.UpdateAsync(user, cancellationToken);
            return Result.Success(new UserResponse(user.Id, user.Name, user.Email, user.Profile.ToString(), user.Active));
        }

        public async Task<Result<IEnumerable<UserResponse>>> GetActiveUsersAsync(CancellationToken cancellationToken = default)
            => await GetUsersAsync(u => u.Active, cancellationToken);

        public async Task<Result<IEnumerable<UserResponse>>> GetUsersAsync(Expression<Func<User, bool>> predicate, CancellationToken cancellationToken = default)
        {
            var result = await repository.GetAllAsync(predicate, cancellationToken);

            var userResponses = result!.Select(user => new UserResponse(user.Id, user.Name, user.Email, user.Profile.ToString(), user.Active));
            return Result.Success(userResponses);
        }

        public async Task<Result> UpdateRoleUserAsync(UpdateRoleRequest request, string correlationId, CancellationToken cancellationToken = default)
        {
            var user = await repository.GetAsync(u => u.Id == request.userId, cancellationToken);

            if (user is null)
                return Result.Failure(new Error("404", "Usuário não encontrado."));

            if (!Enum.TryParse<EProfileType>(request.role.ToString(), true, out var profile))
                return Result.Failure(new Error("400", "Perfil inválido."));

            user.UpdateRole(profile);

            //TODO: Criar evento de usuário ativado
            //TODO: Anexar evento ao eventStore 

            await repository.UpdateAsync(user, cancellationToken);

            return Result.Success(new UserResponse(user.Id, user.Name, user.Email, user.Profile.ToString(), user.Active));
        }
    }
}
