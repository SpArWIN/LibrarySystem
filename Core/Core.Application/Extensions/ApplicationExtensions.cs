using System.Reflection;
using System.Security.Claims;
using System.Text;
using Common.Contracts.Constaints.Sections;
using Common.Contracts.Settings;
using Common.Db.Factory;
using Common.Extensions;
using Common.Http;
using Common.Http.Context;
using Common.Messaging.Nats.Extensions;
using Common.Validation.Api.CustomException;
using Common.Validation.Api.Errors;
using Common.Validation.Extensions;
using Core.Application.Auth.Extensions;
using Core.Application.Builder;
using Core.Application.Services.AuthorizeService;
using Core.Application.Services.Hash;
using Core.Application.Services.JWt;
using Core.Application.Services.Mappings;
using Core.Application.Services.Permissions;
using Core.Application.Services.PreloadedImages;
using Core.Infrastructure;
using Core.Infrastructure.Context;
using Core.Infrastructure.Extensions.CacheExtensions;
using Core.Infrastructure.Extensions.Context;
using Core.Infrastructure.Factory;
using Core.Infrastructure.Tentant;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Core.Application.Extensions;

/// <summary>
/// Расширение на добавление сервисов.
/// </summary>
public static class ApplicationExtensions
{
    /// <summary>
    /// Добавление основных сервисов.
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/>.</param>
    /// <param name="configuration"><see cref="IConfiguration"/>.</param>
    /// <returns><see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        
        services.AddScoped<IPermissionMapper, PermissionMapper>();
        services.AddScoped<IPermissionEvaluator, PermissionEvaluator>();
        services.AddScoped<IClaimBuilder, ClaimBuilder>();
        services.AddScoped<IAuthorizeService, AuthorizeService>();
        
        services.AddSingleton<IPreloadedImageUrlService, PreloadedImageUrlService>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<IJwtTokenService, JwtTokenService>();
        
        services.AddUserPresenceCaching(configuration);
        services.AddContextConfiguration(configuration);
        services.AddErrorHandling(Assembly.GetExecutingAssembly());
        services.AddAuthMediatR();
        services.AddNats(configuration);
        services.AddValidationService([Assembly.GetExecutingAssembly()]);
        return services;
    }

    /// <summary>
    /// Добавление конкретных фабрик.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddUnitOfWorkFactories(this IServiceCollection services)
    {
        services.AddSingleton<IAppDbContextFactory<LibraryDbContext>, LibraryDbContextFactory>();
        services.AddSingleton<IUnitOfWorkFactory<CentralDbContext>, CentralUnitOfWorkFactory>();
        services.AddSingleton<IUnitOfWorkFactory<LibraryDbContext>, UnitOfWorkFactory>();
        return services;
    }
    
    /// <summary>
    /// Добавлене конфигурации получения настроек.
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/>.</param>
    /// <param name="configuration"><see cref="IConfiguration"/>.</param>
    /// <returns><see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddTentantConfiguration(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.ConfigureAndAdd<TenantDatabaseOptions>(configuration.GetSection(Section.TenantDatabase));
        services.AddScoped<ICentralLibraryRegistry, CentralLibraryRegistry>();
        return services;
    }

    /// <summary>
    /// Добавление конфигурации аутентификации.
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/>.</param>
    /// <param name="configuration"><see cref="IConfiguration"/>.</param>
    /// <returns></returns>
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services,
        IConfiguration configuration)
    {
        
        var cfg = BuildPrefixedConfiguration(configuration);
        services.ConfigureAndAdd<RefreshTokenOptions>(cfg.GetSection(Section.RefreshToken));
        services.ConfigureAndAdd<JwtOptions>(cfg.GetSection(Section.Jwt));
        var jwt = BindJwtOptions(configuration);
        
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = jwt.Issuer,
                    ValidAudience = jwt.Audience,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.SigningKey)),
                    ClockSkew = TimeSpan.FromSeconds(30),
                    NameClaimType = ClaimTypes.Name,
                    RoleClaimType = ClaimTypes.Role,
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        if (context.Token.IsNullOrEmpty() && ActorContext.Token.IsNotNullOrEmpty())
                        {
                            context.Token = ActorContext.Token;
                        }

                        return Task.CompletedTask;
                    },
                    OnChallenge = context =>
                    {
                        context.HandleResponse();
                        throw LibraryException.Create(ApiErrors.Auth.Unauthorized);
                    },
                    OnForbidden = _ => throw LibraryException.Create(ApiErrors.Auth.Forbidden),
                };
            });
        
        return services;
    }
    
    private static IConfiguration BuildPrefixedConfiguration(IConfiguration configuration)
        => new ConfigurationBuilder()
            .AddConfiguration(configuration)
            .AddEnvironmentVariables("LIBRARY__")
            .Build();
    
    private static JwtOptions BindJwtOptions(IConfiguration configuration)
    {
        var cfg = BuildPrefixedConfiguration(configuration);

        var jwt = cfg.GetSection(Section.Jwt).Get<JwtOptions>();
        return jwt;
    }
    
}