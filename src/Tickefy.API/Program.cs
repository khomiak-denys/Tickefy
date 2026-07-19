using FluentValidation;
using MediatR;
using Serilog;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Tickefy.API.ErrorHandling;
using Tickefy.API.ErrorHandling.ExceptionMapper;
using Tickefy.API.Options;
using Tickefy.Application.Abstractions.Data;
using Tickefy.Application.Abstractions.Services;
using Tickefy.Application.Auth.Login;
using Tickefy.Application.PipelineBehaviors;
using Tickefy.Infrastructure.Services.AI;
using Tickefy.Domain.Tickets;
using Tickefy.Infrastructure.Database;
using Tickefy.Infrastructure.Options;
using Tickefy.Infrastructure.Repositories;
using Tickefy.Infrastructure.Services;
using Tickefy.Domain.ActivityLogs;
using Tickefy.Domain.Teams;
using Tickefy.Application.Teams.AddMember;
using Tickefy.Domain.Attachments;
using Tickefy.Domain.RefreshTokens;
using Tickefy.Domain.Users;

namespace Tickefy.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Host.UseSerilog((context, services, configuration) => configuration
                //.ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext()
                .WriteTo.Console());

            builder.Services.AddProblemDetails(configure =>
            {
                configure.CustomizeProblemDetails = options =>
                {
                    options.ProblemDetails.Extensions.TryAdd("traceId", System.Diagnostics.Activity.Current?.Id);
                };
            });

            builder.Services.AddSingleton<IExceptionProblemDetailsMapper, ExceptionProblemDetailsMapper>();
            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

            builder.Services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblyContaining<LoginUserCommandHandler>();
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            });

            builder.Services.AddValidatorsFromAssemblyContaining<AddMemberCommandValidator>();

            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IAiService, AiService>();
            builder.Services.AddScoped<IAiResponseParser, AiResponseParser>();
            builder.Services.AddScoped<IObjectStorageService, FakeObjectStorageService>();

            builder.Services.AddScoped<IUserRepository, EFUserRepository>();
            builder.Services.AddScoped<ITicketRepository, EFTicketRepository>();
            builder.Services.AddScoped<IActivityLogRepository, EFLogRepository>();
            builder.Services.AddScoped<ITeamRepository, EFTeamRepository>();
            builder.Services.AddScoped<IRefreshTokenRepository, EFRefreshTokenRepository>();
            builder.Services.AddScoped<IAttachmentRepository, EFAttachmentRepository>();

            builder.Services.AddSingleton(sp =>
            {
                var apiKey = builder.Configuration.GetSection("AiOptions:ApiKey").Value;
                return new Google.GenAI.Client(apiKey: apiKey);
            });

            builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));

            var corsOptions = builder.Configuration
                .GetRequiredSection(CorsOptions.SectionName)
                .Get<CorsOptions>()!;

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

            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                dbContext.Database.Migrate();
            }

            app.UseExceptionHandler();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors(corsOptions.Name);
            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
