using Common.Logging.Extensions;
using Common.Migrator.Extensions;
using Common.Policies.Di;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MigrationService.Service;

namespace MigrationService;

static class Program
{
    static async Task Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .UseSerilog("MigrationService")
            .ConfigureAppConfiguration((_, cfg) =>
            {
                var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
                cfg.AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                    .AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: false)
                    .AddEnvironmentVariables(prefix: "LIBRARY__");
            })
            .ConfigureServices((ctx, services) =>
            {
                services.AddPoliciesService();
                services.AddMigrator(ctx.Configuration);
                services.AddDefaultPolicies();
                services.AddHostedService<MigratorRunner>();
            }).Build();
        
        await host.RunAsync();
    }
}