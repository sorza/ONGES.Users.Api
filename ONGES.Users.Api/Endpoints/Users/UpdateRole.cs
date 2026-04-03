using Microsoft.AspNetCore.Http.HttpResults;
using ONGES.Users.Application.DTOs.Requests;
using ONGES.Users.Application.DTOs.Responses;
using ONGES.Users.Application.Services;

namespace ONGES.Users.Api.Endpoints.Users
{
    public class UpdateRole : IEndpoint
    {
        public static void Map(IEndpointRouteBuilder app)
            => app.MapPut("/role", HandleAsync)
                .WithName("AtualizarPerfil")
                .WithSummary("Atualiza o perfil de um usuário")
                .WithDescription("Atualiza o perfil de um usuário na aplicação.")
                .Produces<Ok<UserResponse>>(200)
                .Produces<NotFound<UserResponse>>(404)
                .Produces<BadRequest<UserResponse>>(400)
                .Produces(401)                  
                .RequireAuthorization("SomenteGestor");

        public static async Task<IResult> HandleAsync(
                        HttpContext httpContext,
                        UpdateRoleRequest request,
                        IUserService userService,
                        CancellationToken cancellationToken = default)
        {
            var correlationId = httpContext.Items["CorrelationId"]?.ToString();
            var result = await userService.UpdateRoleUserAsync(request, correlationId!, cancellationToken);

            var response = result.IsSuccess
                ? Results.Ok(result)
                : result.Error.Code switch
                {
                    "404" => Results.NotFound(result.Error),
                    _ => Results.BadRequest(result.Error)
                };

            return response;

        }

    }
}
