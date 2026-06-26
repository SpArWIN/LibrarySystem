using Common.Localization.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Localization.Extensions;

/// <summary>
/// Расширение на локализацию.
/// </summary>
public static class LocalizationExtensions
{
    /// <summary>
    /// Добавление сервиса локализации.
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/>.</param>
    /// <returns><see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddLibraryLocalization(this IServiceCollection services)
    {
        services.AddSingleton<ILocalizationService, LocalizationService>();
        services.AddSingleton<IErrorLocalization, ErrorLocalization>();
        return services;
    }
}