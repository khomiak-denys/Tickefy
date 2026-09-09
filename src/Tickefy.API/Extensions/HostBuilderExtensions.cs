using System.Text;
using System.Threading.RateLimiting;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Tickefy.API.ErrorHandling;
using Tickefy.API.ErrorHandling.ExceptionMapper;
using Tickefy.API.Options;
using Tickefy.Application.Abstractions.Data;
using Tickefy.Application.Abstractions.Services;
using Tickefy.Application.Auth.Login;
using Tickefy.Application.PipelineBehaviors;
using Tickefy.Domain.ActivityLogs;
using Tickefy.Domain.RefreshTokens;
using Tickefy.Domain.Teams;
using Tickefy.Domain.Tickets;
using Tickefy.Domain.Users;
using Tickefy.Infrastructure.Database;
using Tickefy.Infrastructure.Options;
using Tickefy.Infrastructure.Repositories;
using Tickefy.Infrastructure.Services;
using Tickefy.Infrastructure.Services.AI;

namespace Tickefy.API.Extensions
{
    public static class HostBuilderExtensions
    {
        public static void AddSerilogLogging(this IHostBuilder builder)
        {
            builder.UseSerilog((context, services, configuration) => configuration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext()
                .WriteTo.Console());
        }

        public static void AddErrorHandling(this WebApplicationBuilder builder)
        {
            builder.Services.AddProblemDetails(configure =>
            {
                configure.CustomizeProblemDetails = options =>
                {
                    options.ProblemDetails.Extensions.TryAdd("traceId", System.Diagnostics.Activity.Current?.Id);
                };
            });

            builder.Services.AddSingleton<IExceptionProblemDetailsMapper, ExceptionProblemDetailsMapper>();
            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        }

        public static void AddAuthentication(this WebApplicationBuilder builder)
        {
            builder.Services
                .AddOptions<JwtOptions>()
                .BindConfiguration(JwtOptions.SectionName)
                .ValidateDataAnnotations()
                .ValidateOnStart();

            var jwtOptions = builder.Configuration
                .GetRequiredSection(JwtOptions.SectionName)
                .Get<JwtOptions>()!;

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
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key)),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true
                };
            });
        }

        public static void AddSwagger(this WebApplicationBuilder builder)
        {
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
        }

        public static void AddRateLimiting(this WebApplicationBuilder builder)
        {
            builder.Services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                options.AddTokenBucketLimiter("public-api", opt =>
                {
                    opt.TokenLimit = 10;
                    opt.TokensPerPeriod = 10;
                    opt.ReplenishmentPeriod = TimeSpan.FromSeconds(60);
                    opt.AutoReplenishment = true;
                    opt.QueueLimit = 0;
                    opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                });
            });
        }

        public static void AddCors(this WebApplicationBuilder builder, CorsOptions corsOptions)
        {
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(corsOptions.Name, policy =>
                {
                    policy.WithOrigins(corsOptions.AllowedOrigins)
                          .WithMethods(corsOptions.AllowedMethods)
                          .WithHeaders(corsOptions.AllowedHeaders)
                          .AllowCredentials();
                });
            });
        }

        public static void RegisterInfrastructure(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IAiService, AiService>();
            builder.Services.AddScoped<IAiResponseParser, AiResponseParser>();

            builder.Services.AddScoped<IUserRepository, EFUserRepository>();
            builder.Services.AddScoped<ITicketRepository, EFTicketRepository>();
            builder.Services.AddScoped<IActivityLogRepository, EFLogRepository>();
            builder.Services.AddScoped<ITeamRepository, EFTeamRepository>();
            builder.Services.AddScoped<IRefreshTokenRepository, EFRefreshTokenRepository>();

            builder.Services.AddSingleton(sp =>
            {
                var apiKey = builder.Configuration.GetSection("AiOptions:ApiKey").Value;
                return new Google.GenAI.Client(apiKey: apiKey);
            });

            builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));
        }

        public static void RegisterMediatrPipeline(this WebApplicationBuilder builder)
        {
            builder.Services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblyContaining<LoginUserCommandHandler>();
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            });

            builder.Services.AddValidatorsFromAssemblyContaining<LoginUserCommandValidator>();
        }
    }
}
