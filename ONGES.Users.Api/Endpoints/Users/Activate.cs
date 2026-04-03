using Microsoft.AspNetCore.Http.HttpResults;
using ONGES.Users.Application.DTOs.Responses;
using ONGES.Users.Application.Services;

namespace ONGES.Users.Api.Endpoints.Users
{
    public class Activate : IEndpoint
    {
        public static void Map(IEndpointRouteBuilder app)
        => app.MapPut("/activate/{id:guid}", HandleAsync)
            .WithName("Ativar")
            .WithSummary("Ativa um usuário por ID.")
            .WithDescription("Ativa um usuário por ID na aplicação.")
            .Produces<Ok<UserResponse>>(200)
            .Produces<NotFound<UserResponse>>(404)
            .Produces(401)
            .RequireAuthorization("SomenteGestor");

        public static async Task<IResult> HandleAsync(
                HttpContext context,
                Guid id,
                IUserService service,
                CancellationToken cancellationToken = default)
        {
            var correlationId = context.Items["CorrelationId"]?.ToString();
            var result = await service.ActivateUserAsync(id, correlationId!, cancellationToken);
            return result.IsSuccess
                ? Results.Ok(result)
                : Results.NotFound(result);
        }
    }
}
