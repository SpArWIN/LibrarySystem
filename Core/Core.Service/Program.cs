using System.Reflection;
using Common.Contracts.Settings;
using Common.Db.Extensions;
using Common.Http.Extensions;
using Common.Http.Middleware;
using Common.Logging.Extensions;
using Common.Policies.Di;
using Core.Application.Extensions;
using Core.Infrastructure;
using Core.Infrastructure.Context;
using Core.Infrastructure.Extensions.DataBase;
using Core.Infrastructure.Repository;
using Core.Service.Extensions;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddOpenApi();
builder.Configuration.AddEnvironmentVariables("LIBRARY__");
builder.Host.UseSerilog("Core.Service");
builder.Logging.AddCustomLogging();

var services = builder.Services;
var configuration = builder.Configuration;
services.AddControllers();
services.AddCentralDbContext(configuration);
services.AddCommonDb<CentralDbContext>([typeof(AuthorizationRepository).Assembly]);
    // Регистрируем LibraryDbContext, для того, чтобы фабрика репозиториев знала где искать реализации репозиториев
services.AddCommonDb<LibraryDbContext>([typeof(BookRepository).Assembly]);
services.AddUnitOfWorkFactories();
services.AddTentantConfiguration(configuration);
services.AddTenantProvisioningServices(configuration);
services.AddApplicationServices(configuration);
services.AddJwtAuthentication(configuration);
services.AddPermissionPolicies();
services.AddHttpContextAccessor();
services.AddSwaggerWithXml(Assembly.GetExecutingAssembly(), typeof(JwtOptions).Assembly);
services.AddPoliciesService();
services.AddDefaultPolicies();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwaggerWithUi();
}

app.UseRouting();
app.UseAuthentication();
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<TenantResolutionMiddleware>();
app.UseAuthorization();

app.UseHttpsRedirection();
app.MapControllers();

await app.RunAsync();
