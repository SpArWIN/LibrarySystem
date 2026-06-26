using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Common.Extensions;

/// <summary>
/// Расширение для конфигураций.
/// </summary>
public static class ConfigurationExtensions
{
    /// <summary>
    /// Зарегистрировать настройки и секции конфигурации.
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/>.</param>
    /// <param name="config"><see cref="IConfiguration"/>.</param>
    /// <typeparam name="T">Тип настроек.</typeparam>
    /// <returns><see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection ConfigureAndAdd<T>(
        this IServiceCollection services,
        IConfiguration config)
        where T : class
    {
        services.Configure<T>(config);
        services.AddSingleton(provider => provider.GetRequiredService<IOptions<T>>().Value);
        return services;
    }

    /// <summary>
    /// Зарегистрировать конфигурацию, передав делегат.
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/>.</param>
    /// <param name="config"><see cref="IConfiguration"/>.</param>
    /// <typeparam name="T">Тип настроек.</typeparam>
    /// <returns><see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection ConfigureAndAdd<T>(
        this IServiceCollection services,
        Action<T> config) where T : class
    {
        services.Configure(config);
        services.AddSingleton(provider => provider.GetRequiredService<IOptions<T>>().Value);
        return services;
    }

    /// <summary>
    /// Зарегистрировать конфигурацию и провалидировать её.
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/>.</param>
    /// <param name="config"><see cref="IConfiguration"/>.</param>
    /// <typeparam name="T">Тип настроек.</typeparam>
    /// <returns><see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection ConfigureAndValidate<T>(
        this IServiceCollection services,
        IConfiguration config) where T : class
    {
        services.AddOptions<T>()
            .Bind(config)
            .ValidateDataAnnotations();
        services.AddSingleton(provider => provider.GetRequiredService<IOptions<T>>().Value);
        return services;
    }

    /// <summary>
    ///  Зарегистрировать конфигурацию и провалидировать её собственным валидатором.
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/>.</param>
    /// <param name="config"><see cref="IConfiguration"/>.</param>
    /// <param name="validator">Валидатор.</param>
    /// <param name="errorMessageKey">Ключ сообщения при ошибке по умолчанию.</param>
    /// <typeparam name="T">Тип.</typeparam>
    /// <returns><see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection ConfigureAndValidate<T>(
        this IServiceCollection services,
        IConfiguration config,
        Func<T, bool> validator,
        string errorMessageKey = "Options Validate"
        ) where T : class
    {
        services
            .AddOptions<T>()
            .Bind(config)
            .Validate(validator, errorMessageKey);
        
        services.AddSingleton(provider => provider.GetRequiredService<IOptions<T>>().Value);
        return services;
    }
    
}