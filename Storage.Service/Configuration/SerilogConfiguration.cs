namespace Storage.Service.Configuration;

/// <summary>
/// Конфигурация Serilog
/// </summary>
public static class SerilogConfiguration
{
    /// <summary>
    /// Добавить конфигурация лога.
    /// </summary>
    /// <param name="configurationBuilder"><see cref="IConfigurationBuilder"/>.</param>
    /// <param name="environmentName">Переменная окружения.</param>
    /// <returns></returns>
    public static IConfigurationBuilder AddSerilogConfiguration(
        this IConfigurationBuilder configurationBuilder,
        string environmentName)
    {
        configurationBuilder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true)
            .AddJsonFile("serilog.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();
        return configurationBuilder;
    }
}