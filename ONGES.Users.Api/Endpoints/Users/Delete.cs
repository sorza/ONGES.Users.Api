using Microsoft.AspNetCore.Http.HttpResults;
using ONGES.Users.Application.Services;

namespace ONGES.Users.Api.Endpoints.Users
{
    public class Delete : IEndpoint
    {
        public static void Map(IEndpointRouteBuilder app)
        => app.MapDelete("/{id:guid}", HandleAsync)
            .WithName("Excluir")
            .WithSummary("Exclui um usuário por ID.")
            .WithDescription("Exclui um usuário por ID na aplicação.")
            .Produces(204)
            .Produces<NotFound>(404)
            .AllowAnonymous();

        public static async Task<IResult> HandleAsync(
                HttpContext context,
                Guid id,
                IUserService service,
                CancellationToken cancellationToken = default)
        {
            var correlationId = context.Items["CorrelationId"]?.ToString();
            var result = await service.RemoveUserAsync(id, correlationId!, cancellationToken);
            return result.IsSuccess
                ? Results.NoContent()
                : Results.NotFound(result);
        }
    }
}
