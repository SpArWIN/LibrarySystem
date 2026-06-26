using Common.Http.Context;
using Common.Http.Middleware;
using Microsoft.AspNetCore.Builder;

namespace Common.Http.Extensions;

/// <summary>
/// Подключение <see cref="ActorContext"/> в HTTP-пайплайне.
/// </summary>
public static class ActorContextApplicationBuilderExtensions
{
    /// <summary>
    /// Парсит JWT из Authorization и кладёт в <see cref="Context.ActorContext"/>.
    /// Вызывать до <c>UseAuthentication</c>.
    /// </summary>
    public static IApplicationBuilder UseActorContext(this IApplicationBuilder app) =>
        app.UseMiddleware<ActorMiddleware>();
}
