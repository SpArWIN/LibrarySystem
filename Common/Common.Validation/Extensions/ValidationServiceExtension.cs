using System.Reflection;
using Common.Extensions;
using Common.Localization.Extensions;
using Common.Validation.Api.ErrorHandle;
using Common.Validation.Api.Handle;
using Common.Validation.Pipeline;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Validation.Extensions;

/// <summary>
/// Di для пайплайна валидации.
/// </summary>
public static class ValidationServiceExtension
{
    /// <summary>
    /// Добавление пайплайна валидации.
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/>.</param>
    /// <param name="assemblies">Массив сборок.</param>
    /// <returns></returns>
    public static IServiceCollection AddValidationService(this IServiceCollection services, 
        IEnumerable<Assembly> assemblies)
    {
        services.AddValidatorsFromAssemblies(assemblies);
        services.AddTransient(
            typeof(IPipelineBehavior<,>),
            typeof(ValidationPipelineBehavior<,>));
        
        return services;
    }

    /// <summary>
    /// Добавить обработчик валидации.
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/>.</param>
    /// <param name="additionalAssemblies">Дополнительные обработчики.</param>
    /// <returns><see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddErrorHandling(
        this IServiceCollection services,
        params Assembly[] additionalAssemblies)
    {
        services.AddScoped<IPipelineBehavior<HandleLibraryExceptionContext, HandleErrorResponse>, LibraryExceptionPipeline>();
        services.AddScoped<IPipelineBehavior<HandleValidationExceptionContext, HandleErrorResponse>, HandleValidationExceptionPipeline>();
        
        services.AddLibraryLocalization();
        
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(LibraryExceptionPipeline).Assembly);
            additionalAssemblies.ForEach( assembly =>
            {
                cfg.RegisterServicesFromAssembly(assembly);
            });
        });

        return services;
    }
    
    
}