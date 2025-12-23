using Common.Migrator.Services;
using Core.Infrastructure.Extensions.DataBase;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Common.Migrator;

static class Program
{
    static async Task Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .UseSerilog((ctx, lc) => lc
                .ReadFrom.Configuration(ctx.Configuration)
                .Enrich.FromLogContext()
                .WriteTo.Console())
            .ConfigureAppConfiguration((_, cfg) =>
            {
                var env = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production";
                cfg.AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                    .AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: false)
                    .AddEnvironmentVariables(prefix: "LIBRARY__");
            })
            .ConfigureServices((ctx, services) =>
            {
                services.AddLibraryDbContext(ctx.Configuration);
                services.AddHostedService<MigrationHostedService>();
            })
            .Build();
        
        await host.RunAsync();
    }
    
    
}