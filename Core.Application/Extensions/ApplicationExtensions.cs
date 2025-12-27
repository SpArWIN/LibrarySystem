using System.Text;
using Common.Contracts.Constaints.Sections;
using Common.Contracts.Settings;
using Common.Db.Factory;
using Common.Http;
using Core.Application.Builder;
using Core.Application.Services.AuthorizeService;
using Core.Application.Services.Hash;
using Core.Application.Services.JWt;
using Core.Application.Services.Mappings;
using Core.Infrastructure;
using Core.Infrastructure.Context;
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
        services.AddScoped<IClaimBuilder, ClaimBuilder>();
        services.AddScoped<IAuthorizeService, AuthorizeService>();
        
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<IJwtTokenService, JwtTokenService>();
        services.AddContextConfiguration(configuration);
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
        services.Configure<TenantDatabaseOptions>(configuration.GetSection(Section.TenantDatabase));
        services.AddScoped<ICentralLibraryRegistry, CentralLibraryRegistry>();
        return services;
    }

    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services,
        IConfiguration configuration)
    {
        
        var cfg = BuildPrefixedConfiguration(configuration);
        services.Configure<RefreshTokenOptions>(cfg.GetSection(Section.RefreshToken));
        services.Configure<JwtOptions>(cfg.GetSection(Section.Jwt));
        var jwt = BindJwtOptions(configuration);

        if (string.IsNullOrWhiteSpace(jwt.SigningKey) || jwt.SigningKey.Length < 32)
            throw new ApplicationException("Jwt:SigningKey is missing or too short (min 32 chars).");
        
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
        if (jwt is null
            || string.IsNullOrWhiteSpace(jwt.Issuer)
            || string.IsNullOrWhiteSpace(jwt.Audience)
            || string.IsNullOrWhiteSpace(jwt.SigningKey)
            || jwt.SigningKey.Length < 32
            || jwt.AccessTokenLifetimeMinutes <= 0)
            throw new ApplicationException("Jwt options are missing or invalid.");

        return jwt;
    }
        
}