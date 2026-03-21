using Common.Http.Accessors;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Http.Extensions;

/// <summary>
/// Http расширения.
/// </summary>
public static class HttpExtension
{
    /// <summary>
    /// Добавить Мидлваре.
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/>.</param>
    /// <returns><see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddHttpAccessor(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<ITenantContextAccessor, TenantContextAccessor>();
        services.AddScoped<ICorrelationContextAccessor, CorrelationContextAccessor>();
       // services.AddScoped<ICentralLibraryRegistry,>();
        return services;
    }
}