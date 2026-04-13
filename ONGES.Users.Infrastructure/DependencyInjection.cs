using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using ONGES.Users.Application.DTOs.Requests;
using ONGES.Users.Application.Events;
using ONGES.Users.Application.Repositories;
using ONGES.Users.Application.Services;
using ONGES.Users.Infrastructure.Data;
using ONGES.Users.Infrastructure.Events;
using ONGES.Users.Infrastructure.Repositories;
using ONGES.Users.Infrastructure.Services;
using ONGES.Users.Infrastructure.Validators;

namespace ONGES.Users.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            var mongoString = configuration["MongoSettings:ConnectionString"];
            var mongoDb = configuration["MongoSettings:Database"];
            var mongoCollection = configuration["MongoSettings:Collection"];

            var mongoSettings = MongoClientSettings.FromConnectionString(mongoString);
            mongoSettings.ServerSelectionTimeout = TimeSpan.FromSeconds(5);

            services.AddSingleton<IMongoClient>(sp => new MongoClient(mongoSettings));

            services.AddScoped<IEventStore>(sp =>
            {
                var client = sp.GetRequiredService<IMongoClient>();
                return new MongoEventStore(client, mongoDb!, mongoCollection!);
            });

            services.AddScoped<IValidator<UserRequest>, UserRequestValidator>();
            services.AddScoped<IValidator<AuthRequest>, AuthRequestValidator>();

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserService, UserService>();
            services.AddSingleton<IJwtTokenService, JwtTokenService>();

            return services;
        }
    }
}
