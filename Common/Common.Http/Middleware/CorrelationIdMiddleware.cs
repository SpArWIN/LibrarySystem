using Common.Http.Accessors;
using Microsoft.AspNetCore.Http;
using Serilog.Context;

namespace Common.Http.Middleware;

// app.UseMiddleware<CorrelationIdMiddleware>();
public sealed class CorrelationIdMiddleware(RequestDelegate next)
{
    private const string HeaderName = "X-Correlation-Id";
    public async Task InvokeAsync(
        HttpContext context,
        ICorrelationContextAccessor accessor)
    {
        
        var correlationId =
            context.Request.Headers.TryGetValue(HeaderName, out var values)
            && Guid.TryParse(values.FirstOrDefault(), out var parsed)
                ? parsed
                : Guid.NewGuid();

        accessor.CorrelationContext = new CorrelationContext(correlationId);

        context.Response.Headers[HeaderName] = correlationId.ToString();
        try
        {
            using (LogContext.PushProperty("CorrelationId", correlationId))
            {
                await next(context);
            }
        }
        finally
        {
            accessor.CorrelationContext = null;
        }
    }
}

/// <inheritdoc />
public sealed record CorrelationContext(Guid CorrelationId) : ICorrelationContext
{
    /// <inheritdoc />
    public IDictionary<string, object> Items { get; } = new Dictionary<string, object>();
}