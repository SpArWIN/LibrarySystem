using Common.Policies.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Policies.Di;

/// <summary>
/// Расширение на политику.
/// </summary>
public static class PolicyExtensions
{
    /// <summary>
    /// Добавить сервисы политик.
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/>.</param>
    /// <returns><see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddPoliciesService(this IServiceCollection services)
    {
        services.AddTransient<IDbResilience, DbResilience>();
        return services;
    }
}