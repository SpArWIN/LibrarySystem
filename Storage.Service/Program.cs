using Common.Policies.Di;
using Storage.Application.Extensions;
using Storage.Service.Configuration;
using Storage.Service.Extensions;
using Storage.Service.Initializer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddStorage(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddHealthChecks(); 
builder.Services.AddHostedService<MiniOInitializer>();

builder.Configuration.AddSerilogConfiguration(builder.Environment.EnvironmentName);
builder.ConfigureSerilog();
builder.Logging.AddCustomLogging();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseRouting();
app.MapHealthChecks("/health");
app.MapControllers();
await app.RunAsync();

