using Microsoft.AspNetCore.Http.HttpResults;
using System.Security.Claims;

namespace ONGES.Users.Api.Endpoints.Users
{
    public class AuthCheck : IEndpoint
    {
        public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/auth-check", HandleAsync)
            .WithName("Verificação de Autenticação")
            .WithSummary("Verifica se o usuário está autenticado.")
            .WithDescription("Verifica se o usuário está autenticado na aplicação.")
            .Produces<Ok>(200)
            .Produces(401)
            .RequireAuthorization();

        private static IResult HandleAsync(HttpContext httpContext)
        {
            var user = httpContext.User;

            if (user.Identity is not { IsAuthenticated: true })
                return Results.Unauthorized();

            var userId = user.FindFirstValue("UserId");
            var email = user.FindFirstValue(ClaimTypes.Email);
            var name = user.FindFirstValue("Name");
            var role = user.FindFirstValue(ClaimTypes.Role);

            return Results.Ok(new { Id = userId!, Email = email!, Name = name!, Role = role! });
        }
    }
}
