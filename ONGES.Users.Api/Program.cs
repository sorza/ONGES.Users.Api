using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using ONGES.Users.Api.Endpoints;
using ONGES.Users.Api.Middlewares;
using ONGES.Users.Infrastructure;
using ONGES.Users.Infrastructure.Data;
using Prometheus;
using Scalar.AspNetCore;

namespace ONGES.Users.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddInfrastructure(builder.Configuration);

            builder.Services.AddOpenApi(options =>
            {
                options.AddDocumentTransformer((document, context, ct) =>
                {
                    document.Info = new OpenApiInfo
                    {
                        Title = "ONGES.Users.Api",
                        Version = "v1"
                    };

                    document.Components ??= new OpenApiComponents();
                    document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
                    document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer",
                        BearerFormat = "JWT",
                        Description = "Digite seu token"
                    };

                    var requirement = new OpenApiSecurityRequirement
                    {
                        { new OpenApiSecuritySchemeReference("Bearer"), new List<string>() }
                    };

                    if (document.Paths is not null)
                    {
                        foreach (var path in document.Paths.Values)
                        {
                            foreach (var operation in path.Operations.Values)
                            {
                                operation.Security ??= [];
                                operation.Security.Add(requirement);
                            }
                        }
                    }

                    return Task.CompletedTask;
                });
            });

            builder.Services.AddHttpContextAccessor();

            builder.Services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],

                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["Jwt:Audience"],

                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                    Convert.FromBase64String(builder.Configuration["Jwt:Key"]!))
                };
            });

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("SomenteGestor", policy =>
                    policy.RequireRole("Gestor"));
            });

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                using (var scope = app.Services.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    var retries = 5;
                    while (retries > 0)
                    {
                        try
                        {
                            db.Database.Migrate();
                            break;
                        }
                        catch
                        {
                            retries--;
                            Thread.Sleep(2000);
                        }
                    }
                }
                app.MapOpenApi();
                app.MapScalarApiReference(options =>
                {
                    options.WithTitle("ONGES.Users.Api");
                    options.WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
                });
            }

            app.UseMiddleware<CorrelationIdMiddleware>();
            app.UseMiddleware<GlobalExceptionMiddleware>();

            app.UseHttpsRedirection();
            app.UseHttpMetrics();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapGet("/health", () => Results.Ok("Healthy"));
            app.MapMetrics();

            app.MapEndpoints();

            app.Run();
        }
    }
}
