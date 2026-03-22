using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

namespace Common.Logging.Extensions;

/// <summary>
/// Расщирение регистрации логгирования.
/// </summary>
public static class ServiceExtensions
{
    /// <summary>
    /// Добавить конфигурацию Serilog для IHostBuilder.
    /// </summary>
    /// <param name="hostBuilder">IHostBuilder.</param>
    /// <param name="serviceName">Название сервиса.</param>
    /// <param name="configure">Дополнительная конфигурация.</param>
    /// <returns>IHostBuilder.</returns>
    public static IHostBuilder UseSerilog(
        this IHostBuilder hostBuilder,
        string serviceName,
        Action<HostBuilderContext, LoggerConfiguration>? configure = null)
    {
        
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Application", serviceName)
            .CreateBootstrapLogger();

        try
        {
            hostBuilder.UseSerilog((context, services, configuration) =>
            {
                var env = context.HostingEnvironment;

                configuration
                    .ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(services)
                    .Enrich.FromLogContext()
                    .Enrich.WithProperty("Application", serviceName)
                    .Enrich.WithProperty("Environment", env.EnvironmentName)
                    .MinimumLevel.Debug()
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                    .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
                    .MinimumLevel.Override("System", LogEventLevel.Warning);

               
                configuration.WriteTo.Console(
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {SourceContext} {Message:lj}{NewLine}{Exception}");

                
                if (!env.IsDevelopment())
                {
                    var filePath = $"logs/{serviceName}-.log";
                    configuration.WriteTo.File(
                        path: filePath,
                        rollingInterval: RollingInterval.Day,
                        retainedFileCountLimit: 31,
                        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {SourceContext} {Message:lj}{NewLine}{Exception}",
                        shared: true);
                }

                
                configure?.Invoke(context, configuration);
            });

            return hostBuilder;
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Failed to configure Serilog");
            throw;
        }
    }
    
    /// <summary>
    /// Добавить кастомную конфигурацию логгирования.
    /// </summary>
    public static ILoggingBuilder AddCustomLogging(this ILoggingBuilder loggingBuilder)
    {
        loggingBuilder.ClearProviders();
        loggingBuilder.AddSerilog(dispose: true);
        return loggingBuilder;
    }

}