using Microsoft.EntityFrameworkCore;
using ONGES.Users.Infrastructure;
using ONGES.Users.Infrastructure.Data;

namespace ONGES.Users.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddInfrastructure(builder.Configuration);
          
            builder.Services.AddAuthorization();

            builder.Services.AddSwaggerGen();
            builder.Services.AddOpenApi();

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
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapGet("/", () => "Api On!");

            app.Run();
        }
    }
}
