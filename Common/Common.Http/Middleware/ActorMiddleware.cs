using Common.Http.Context;
using Microsoft.AspNetCore.Http;

namespace Common.Http.Middleware;

/// <summary>
/// Парсит JWT из запроса и кладёт в <see cref="ActorContext"/> на время обработки.
/// </summary>
public sealed class ActorMiddleware
{
    private readonly RequestDelegate _next;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="next">Следующий делегат цепочки.</param>
    public ActorMiddleware(RequestDelegate next) => _next = next;

    
    public async Task InvokeAsync(HttpContext context)
    {
        var token = BearerTokenReader.TryReadFromHeaders(context.Request.Headers);

        using (ActorContext.Set(token))
        {
            await _next(context);
        }
    }
}