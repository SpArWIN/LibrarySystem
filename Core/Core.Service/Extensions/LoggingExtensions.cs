using Serilog;
using Serilog.Events;

namespace Core.Service.Extensions;

/// <summary>
/// Расширение на логгирование.
/// </summary>
public static class LoggingExtensions
{
    /// <summary>
    /// Добавить конфигурациию.
    /// </summary>
    /// <param name="builder"><see cref="WebApplicationBuilder"/>.</param>
    /// <returns></returns>
    public static WebApplicationBuilder ConfigureSerilog(this WebApplicationBuilder builder)
    {
      
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Application", "Core.Service")
            .CreateBootstrapLogger();

        try
        {
            builder.Host.UseSerilog(ConfigureSerilogLogger);

            return builder;
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Failed to configure Serilog");
            throw;
        }
    }
    private static void ConfigureSerilogLogger(
        HostBuilderContext context,
        IServiceProvider services,
        LoggerConfiguration configuration)
    {
        var env = context.HostingEnvironment;
        var config = context.Configuration;

        configuration
            .ReadFrom.Configuration(config)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Application", "Core.Service")
            .Enrich.WithProperty("Environment", env.EnvironmentName)
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .MinimumLevel.Override("System", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information);

     
        if (!env.IsDevelopment())
        {
            configuration.WriteTo.File(
                path: "logs/core-.log",
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 31,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {SourceContext} {Message:lj}{NewLine}{Exception}",
                shared: true);
        }
        
    }
    
    /// <summary>
    /// Добавить кастомную конфигурацию.
    /// </summary>
    /// <param name="loggingBuilder"><see cref="ILoggingBuilder"/>.</param>
    /// <returns></returns>
    public static ILoggingBuilder AddCustomLogging(this ILoggingBuilder loggingBuilder)
    {
        loggingBuilder.ClearProviders();
        loggingBuilder.AddSerilog(dispose: true);
        return loggingBuilder;
    }
    
}