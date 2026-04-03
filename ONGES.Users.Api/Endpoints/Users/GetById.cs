using Microsoft.AspNetCore.Http.HttpResults;
using ONGES.Users.Application.DTOs.Responses;
using ONGES.Users.Application.Services;

namespace ONGES.Users.Api.Endpoints.Users
{
    public class GetById : IEndpoint
    {
        public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/{id:guid}", HandleAsync)
            .WithName("Obter por ID")
            .WithSummary("Obtém um usuário por ID.")
            .WithDescription("Obtém um usuário por ID na aplicação.")
            .Produces<Ok<UserResponse>>(200)
            .Produces<NotFound<UserResponse>>(404)
            .AllowAnonymous();

        public static async Task<IResult> HandleAsync(
                Guid id,
                IUserService service,
                CancellationToken cancellationToken = default)
        {
            var result = await service.GetUserAsync(u => u.Id == id, cancellationToken);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.NotFound(result);
        }
    }
}
