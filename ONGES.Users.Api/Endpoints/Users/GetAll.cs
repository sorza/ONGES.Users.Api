using Microsoft.AspNetCore.Http.HttpResults;
using ONGES.Users.Application.DTOs.Responses;
using ONGES.Users.Application.Services;

namespace ONGES.Users.Api.Endpoints.Users
{
    public class GetAll : IEndpoint
    {
        public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/", HandleAsync)
            .WithName("Obter todos")
            .WithSummary("Obtém todos os usuários.")
            .WithDescription("Obtém todos os usuários na aplicação.")
            .Produces<Ok<List<UserResponse>>>(200)
            .RequireAuthorization("SomenteGestor");

        public static async Task<IResult> HandleAsync(
                IUserService service,
                CancellationToken cancellationToken = default)
        {
            var result = await service.GetAllUsersAsync(cancellationToken);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.BadRequest(result);
        }
    }
}
