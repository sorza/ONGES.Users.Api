using Microsoft.AspNetCore.Http.HttpResults;
using ONGES.Users.Application.DTOs.Responses;
using ONGES.Users.Application.Services;

namespace ONGES.Users.Api.Endpoints.Users
{
    public class Deactivate : IEndpoint
    {
        public static void Map(IEndpointRouteBuilder app)
        => app.MapDelete("/deactivate/{id:guid}", HandleAsync)
            .WithName("Desativar")
            .WithSummary("Desativa um usuário.")
            .WithDescription("Desativa um usuário na aplicação.")
            .Produces<NoContent>(204)
            .Produces<NotFound<UserResponse>>(404)
            .RequireAuthorization("SomenteGestor");

        public static async Task<IResult> HandleAsync(
                HttpContext context,
                Guid id,
                IUserService service,
                CancellationToken cancellationToken = default)
        {
            var correlationId = context.Items["CorrelationId"]?.ToString();
            var result = await service.DeactivateUserAsync(id, correlationId!, cancellationToken);

            return result.IsSuccess
                ? Results.NoContent()
                : Results.NotFound(result);
        }
    }
}
