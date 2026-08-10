using Tickefy.API.Extensions;
using Tickefy.API.Options;

namespace Tickefy.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Host.AddSerilogLogging();

            builder.AddErrorHandling();

            builder.RegisterMediatrPipeline();
            builder.RegisterInfrastructure();

            var corsOptions = builder.Configuration
                .GetRequiredSection(CorsOptions.SectionName)
                .Get<CorsOptions>()!;

            builder.AddCors(corsOptions);
            builder.AddRateLimiting();
            builder.AddAuthentication();

            builder.Services.AddAuthorization();

            builder.Services.AddControllers();
            builder.Services.AddHttpContextAccessor();

            builder.Services.AddEndpointsApiExplorer();
            builder.AddSwagger();

            var app = builder.Build();

            app.ApplyMigrations();

            app.UseExceptionHandler();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors(corsOptions.Name);
            app.UseHttpsRedirection();
            app.UseRateLimiter();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers().RequireRateLimiting("public-api");

            app.Run();
        }
    }
}
