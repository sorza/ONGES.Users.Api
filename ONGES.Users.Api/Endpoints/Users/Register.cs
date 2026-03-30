using Azure;
using Microsoft.AspNetCore.Http.HttpResults;
using ONGES.Users.Application.DTOs.Requests;
using ONGES.Users.Application.DTOs.Responses;
using ONGES.Users.Application.Services;

namespace ONGES.Users.Api.Endpoints.Users
{
    public class Register : IEndpoint
    {
        public static void Map(IEndpointRouteBuilder app)
        => app.MapPost("/", HandleAsync)
            .WithName("Registro")
            .WithSummary("Registra um novo usuário.")
            .WithDescription("Registra um novo usuário na aplicação.")
            .Produces<Created<UserResponse>>(201)
            .Produces<BadRequest<Response<UserResponse>>>(400)
            .AllowAnonymous();

        private static async Task<IResult> HandleAsync(
                HttpContext httpContext, 
                IUserService service, 
                UserRequest userDto, 
                CancellationToken cancellationToken = default)
        {
            var correlationId = httpContext.Items["CorrelationId"]?.ToString();

            var result = await service.AddUserAsync(userDto, correlationId!,cancellationToken);

            return result.IsSuccess
                 ? Results.Created($"/users/{result.Value.Id}", result.Value)
                 : Results.BadRequest(result);
        }
    }
}
