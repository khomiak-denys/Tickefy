using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Tickefy.Infrastructure.Database;



public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {

        DotNetEnv.Env.Load("../../.env");


        var config = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../Tickefy.API"))
            .AddJsonFile("appsettings.json", optional: false)
            .AddEnvironmentVariables()
            .Build();


        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        var connectionString =
                $"Host={Environment.GetEnvironmentVariable("DB_HOST")};" +
                $"Port={Environment.GetEnvironmentVariable("DB_PORT")};" +
                $"Database={Environment.GetEnvironmentVariable("DB_NAME")};" +
                $"Username={Environment.GetEnvironmentVariable("DB_USER")};" +
                $"Password={Environment.GetEnvironmentVariable("DB_PASSWORD")}";

        optionsBuilder.UseNpgsql(connectionString);

        return new AppDbContext(optionsBuilder.Options);
    }
}