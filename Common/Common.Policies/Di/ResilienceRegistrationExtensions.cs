using System.Data.Common;
using System.Net.Sockets;
using Common.Policies.Models;
using Common.Policies.PipelineNames;
using Common.Policies.Pollicies.Extension;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NATS.Client.Core;
using NATS.Client.JetStream;
using Polly;
using Polly.CircuitBreaker;
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

        services.AddResiliencePipeline(NamesPipeline.DbPipelines.Connect, builder =>
        {
            builder.AddJitterPolicy(
                new RetryOptions()
                {
                    MaxRetryAttempts = 5,
                    BaseDelay = TimeSpan.FromMilliseconds(300),
                    Backoff = DelayBackoffType.Exponential,
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

    public static IServiceCollection AddNatsPolicies(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddResiliencePipeline(NamesPipeline.Nats.Publish, builder =>
        {
            builder.AddJitterPolicy(new RetryOptions()
            {
                MaxRetryAttempts = 5,
                BaseDelay = TimeSpan.FromMilliseconds(100),
                Backoff = DelayBackoffType.Exponential,
                UseJitter = true,
            }, IsNatsTransient);
            builder.AddTimeout(TimeSpan.FromSeconds(10));
        });
        
        services.AddResiliencePipeline(NamesPipeline.Nats.Request, builder =>
        {
            builder.AddJitterPolicy(
                new RetryOptions
                {
                    MaxRetryAttempts = 2,
                    BaseDelay = TimeSpan.FromMilliseconds(200),
                    Backoff = DelayBackoffType.Linear,
                    UseJitter = true
                },
                IsNatsTransient);
            
            builder.AddTimeout(TimeSpan.FromSeconds(30));
        });
        
        services.AddResiliencePipeline(NamesPipeline.Nats.Connect, builder =>
        {
            builder.AddJitterPolicy(
                new RetryOptions
                {
                    MaxRetryAttempts = 10,
                    BaseDelay = TimeSpan.FromSeconds(1),
                    Backoff = DelayBackoffType.Exponential,
                    UseJitter = true,
                },
                IsNatsConnectionTransient);
            
            builder.AddCircuitBreaker(new CircuitBreakerStrategyOptions
            {
                FailureRatio = 0.5,
                SamplingDuration = TimeSpan.FromSeconds(30),
                MinimumThroughput = 5,
                BreakDuration = TimeSpan.FromSeconds(60),
                ShouldHandle = new PredicateBuilder()
                    .Handle<NatsException>()
                    .Handle<TimeoutException>()
                    .Handle<SocketException>()
                    .Handle<IOException>()
            });
        });
        
        services.AddResiliencePipeline(NamesPipeline.Nats.JetStream, builder =>
        {
            builder.AddJitterPolicy(
                new RetryOptions
                {
                    MaxRetryAttempts = 5,
                    BaseDelay = TimeSpan.FromMilliseconds(300),
                    Backoff = DelayBackoffType.Exponential,
                    UseJitter = true
                },
                IsJetStreamTransient);
            
            builder.AddTimeout(TimeSpan.FromSeconds(15));
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
    
    private static bool IsNatsConnectionTransient(Exception exception)
    {
        return exception is 
            NatsException or
            NatsJSConnectionException or 
            SocketException or 
            IOException or 
            TimeoutException;
    }
    
    private static bool IsNatsTransient(Exception exception)
    {
        return exception switch
        {
            NatsException natsEx => 
                natsEx.Message.Contains("timeout", StringComparison.OrdinalIgnoreCase) ||
                natsEx.Message.Contains("connection", StringComparison.OrdinalIgnoreCase) ||
                natsEx.Message.Contains("no responders", StringComparison.OrdinalIgnoreCase),
            
            SocketException or 
                IOException or 
                TimeoutException => true,
            
            _ => false
        };
    }
    
    private static bool IsJetStreamTransient(Exception exception)
    {
        return exception switch
        {
            NatsJSException jsEx => 
                jsEx.Message.Contains("timeout", StringComparison.OrdinalIgnoreCase) ||
                jsEx.Message.Contains("no stream", StringComparison.OrdinalIgnoreCase),
            
            _ => IsNatsTransient(exception)
        };
    }
    
    private static bool IsHttpTransient(Exception exception) =>
        exception is TimeoutException or IOException or HttpRequestException;
    
    private static bool IsIoTransient(Exception ex) =>
        ex is TimeoutException or IOException;
}