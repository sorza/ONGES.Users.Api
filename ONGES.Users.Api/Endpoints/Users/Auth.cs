using Microsoft.AspNetCore.Http.HttpResults;
using ONGES.Users.Application.DTOs.Requests;
using ONGES.Users.Application.DTOs.Responses;
using ONGES.Users.Application.Services;

namespace ONGES.Users.Api.Endpoints.Users
{
    public class Auth : IEndpoint
    {
        public static void Map(IEndpointRouteBuilder app)
        => app.MapPost("/auth", HandleAsync)
            .WithName("Autenticação")
            .WithSummary("Autentica um usuário.")
            .WithDescription("Autentica um usuário na aplicação.")
            .Produces<Ok<UserResponse>>(200)
            .Produces<BadRequest<UserResponse>>(400)
            .AllowAnonymous();

        private static async Task<IResult> HandleAsync(
                HttpContext httpContext, 
                IUserService service, 
                AuthRequest authDto, 
                CancellationToken cancellationToken = default)
        {
                var correlationId = httpContext.Items["CorrelationId"]?.ToString();
                var ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
                var device = httpContext.Request.Headers["User-Agent"].ToString() ?? "Unknown";
    
                var result = await service.AuthAsync(authDto, ip, device, correlationId!, cancellationToken);
    
                return result.IsSuccess
                    ? Results.Ok(result.Value)
                    : Results.BadRequest(result);
        }

    }
}
