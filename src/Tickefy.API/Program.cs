using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Tickefy.API.ErrorHandling;
using Tickefy.API.ErrorHandling.ExceptionMapper;
using Tickefy.API.Mapping;
using Tickefy.Application.Abstractions.Data;
using Tickefy.Application.Abstractions.Repositories;
using Tickefy.Application.Abstractions.Services;
using Tickefy.Application.Auth.Login;
using Tickefy.Application.Common.Mapping;
using Tickefy.Application.PipelineBehaviors;
using Tickefy.Infrastructure.Services.AI;
using Tickefy.Domain.Ticket;
using Tickefy.Infrastructure.Database;
using Tickefy.Infrastructure.Options;
using Tickefy.Infrastructure.Repositories;
using Tickefy.Infrastructure.Services;
using Tickefy.Domain.ActivityLog;
using Tickefy.Domain.Team;

namespace Tickefy.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            builder.Services.AddProblemDetails(configure =>
            {
                configure.CustomizeProblemDetails = options =>
                {
                    options.ProblemDetails.Extensions.TryAdd("traceId", System.Diagnostics.Activity.Current?.Id);
                };
            });

            builder.Services.AddSingleton<IExceptionProblemDetailsMapper, ExceptionProblemDetailsMapper>();
            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();


            if (builder.Environment.IsDevelopment())
            {
                DotNetEnv.Env.Load("../../.env");
            }

            builder.Services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblyContaining<LoginUserCommandHandler>();
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            });

            builder.Services.AddValidatorsFromAssemblyContaining<LoginUserCommandValidator>();

            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IAiService, AiService>();
            builder.Services.AddScoped<IAiResponseParser, AiResponseParser>();

            builder.Services.AddScoped<IUserRepository, EFUserRepository>();
            builder.Services.AddScoped<ITicketRepository, EFTicketRepository>();
            builder.Services.AddScoped<IActivityLogRepository, EFLogRepository>();
            builder.Services.AddScoped<ITeamRepository, EFTeamRepository>();


            builder.Services.AddSingleton(sp =>
            {
                var apiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY");
                return new Google.GenAI.Client(apiKey: apiKey);
            });


            builder.Services.AddAutoMapper(
                cfg => 
                {
                    cfg.AllowNullCollections = true;
                },
                new[]
                {
                    typeof(LoginMappingProfile).Assembly,
                    typeof(TicketProfile).Assembly 

                }
            ); 

            var postgresConnection =
                $"Host={Environment.GetEnvironmentVariable("DB_HOST")};" +
                $"Port={Environment.GetEnvironmentVariable("DB_PORT")};" +
                $"Database={Environment.GetEnvironmentVariable("DB_NAME")};" +
                $"Username={Environment.GetEnvironmentVariable("DB_USER")};" +
                $"Password={Environment.GetEnvironmentVariable("DB_PASSWORD")}";


            var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY");
            var jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER");
            var jwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE");
            var jwtValidityMins = int.Parse((Environment.GetEnvironmentVariable("JWT_VALIDITY_MINS") ?? "30"));

            builder.Services.Configure<JwtSettings>(options =>
            {
                options.Key = jwtKey!;
                options.Issuer = jwtIssuer!;
                options.Audience = jwtAudience!;
                options.TokenValidityMins = jwtValidityMins;
            });

            builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseNpgsql(postgresConnection));

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy.WithOrigins("http://localhost:3000", "http://127.0.0.1:3000")
                        .WithMethods("GET", "POST", "PUT", "DELETE", "OPTIONS")
                        .WithHeaders("Content-Type", "Authorization")
                        .AllowCredentials();
                });
            });

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = jwtIssuer,
                    ValidAudience = jwtAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey!)),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true
                };
            });
            builder.Services.AddAuthorization();

            builder.Services.AddControllers();
            builder.Services.AddHttpContextAccessor();

            builder.Services.AddEndpointsApiExplorer();


            builder.Services.AddSwaggerGen(options =>
            {
                var jwtSecurityScheme = new OpenApiSecurityScheme
                {
                    BearerFormat = "JWT",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = JwtBearerDefaults.AuthenticationScheme,
                    Description = "Enter your JWT access token",
                    Reference = new OpenApiReference
                    {
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        Type = ReferenceType.SecurityScheme
                    }
                };

                options.AddSecurityDefinition("Bearer", jwtSecurityScheme);
                options.EnableAnnotations();
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    { jwtSecurityScheme, Array.Empty<string>() }
                });
            });

            var app = builder.Build();

            app.UseExceptionHandler();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();
           
            app.MapControllers();

            app.Run();
        }
    }
}
