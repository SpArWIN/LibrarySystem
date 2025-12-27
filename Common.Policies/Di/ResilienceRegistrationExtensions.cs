using System.Data.Common;
using System.Net.Sockets;
using Common.Policies.Models;
using Common.Policies.PipelineNames;
using Common.Policies.Pollicies.Extension;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Registry;

namespace Common.Policies.Di;

/// <summary>
/// Расширение на регистрацию политик.
/// </summary>
public static class ResilienceRegistrationExtensions
{
    /// <summary>
    /// Добавление дефолтных политик повторений.
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/>.</param>
    /// <returns><see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddDefaultPolicies(this IServiceCollection services)
    {
        services.AddResiliencePipeline(NamesPipeline.DbPipelines.Write, builder =>
        {
            builder.AddJitterPolicy(
                new RetryOptions()
                {
                    MaxRetryAttempts = 5,
                    BaseDelay = TimeSpan.FromMilliseconds(500),
                    Backoff = DelayBackoffType.Exponential,
                    UseJitter = true
                },
                IsEfTransient
            );
        });

        services.AddResiliencePipeline(NamesPipeline.DbPipelines.Read, builder =>
        {
            builder.AddJitterPolicy(
                new RetryOptions()
                {
                    MaxRetryAttempts = 3,
                    BaseDelay = TimeSpan.FromMilliseconds(300),
                    Backoff = DelayBackoffType.Linear,
                    UseJitter = true
                },
                IsEfTransient
            );
        });
        
        services.AddResiliencePipeline(NamesPipeline.Http.Outbound, builder =>
        {
            builder.AddJitterPolicy(
                new RetryOptions { 
                    MaxRetryAttempts = 5, 
                    BaseDelay = TimeSpan.FromMilliseconds(200), 
                    Backoff = DelayBackoffType.Exponential,
                    UseJitter = true },
                IsHttpTransient);
        });
        
        services.AddResiliencePipeline(NamesPipeline.Storage.Io, builder =>
        {
            builder.AddJitterPolicy(
                new RetryOptions
                {
                    MaxRetryAttempts = 3,
                    BaseDelay = TimeSpan.FromMilliseconds(150), 
                    Backoff = DelayBackoffType.Constant,
                    UseJitter = true
                },
                IsIoTransient);
        });
        return services;
        
        
    }
    /// <summary>Получить пайплайн по имени.</summary>
    public static ResiliencePipeline GetPipeline(
        this ResiliencePipelineProvider<string> provider, string name)
        => provider.GetPipeline(name);
    
    private static bool IsEfTransient(Exception exception) =>
        exception is TimeoutException
        || exception is IOException
        || exception is SocketException
        || exception.InnerException is DbException;
    
    private static bool IsHttpTransient(Exception exception) =>
        exception is TimeoutException or IOException or HttpRequestException;
    
    private static bool IsIoTransient(Exception ex) =>
        ex is TimeoutException or IOException;
}