using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Common.Contracts.Claims;
using Common.Contracts.Settings;
using Common.Http.Accessors;
using Common.Http.Extensions;
using Microsoft.AspNetCore.Http;

namespace Common.Http.Middleware;


// Добавление

/*app.UseRouting();

app.UseAuthentication();
app.UseMiddleware<TenantResolutionMiddleware>();
app.UseAuthorization();*/
public sealed class TenantResolutionMiddleware (RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, 
        ITenantContextAccessor accessor,
        ICentralLibraryRegistry registry
      )
    {
        var cancellationToken = context.RequestAborted;
        var libraryId = context.User.TryGetLibraryIdFromClaims()
                        ?? context.Request.TryGetLibraryIdFromHeader();
        if (libraryId is null)
        {
            await next(context);
            return;
        }
        var dbSettings =  await registry.GetDbSettingsAsync(libraryId.Value, cancellationToken);
        accessor.CurrentContext = new TenantContext(libraryId.Value, dbSettings);
        await next(context);
    }
    
    
    
    private sealed class TenantContext(Guid libraryId, DataBaseSettings dbSettings) : ITenantContext
    {
        public Guid LibraryId { get; } = libraryId;
        public DataBaseSettings DbSettings { get; } = dbSettings;
    }
}