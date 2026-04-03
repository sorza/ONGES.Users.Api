using ONGES.Users.Api.Endpoints.Users;

namespace ONGES.Users.Api.Endpoints
{
    public static class Endpoint
    {
        public static void MapEndpoints(this WebApplication app)
        {
            var endpoints = app.MapGroup("");

            endpoints.MapGroup("v1/auth")
                .WithTags("Autenticação")
                .MapEndpoint<Auth>()
                .MapEndpoint<AuthCheck>();                

             endpoints.MapGroup("v1/users")
                .WithTags("Usuários")
                .MapEndpoint<Register>()
                .MapEndpoint<GetById>()
                .MapEndpoint<GetAll>()
                .MapEndpoint<UpdateRole>()
                .MapEndpoint<Activate>()
                .MapEndpoint<Deactivate>()
                .MapEndpoint<Delete>();
        }

        private static IEndpointRouteBuilder MapEndpoint<TEndpoint>(this IEndpointRouteBuilder app)
           where TEndpoint : IEndpoint
        {
            TEndpoint.Map(app);
            return app;
        }
    }
}
