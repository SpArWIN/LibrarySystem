using Common.Validation.Api.CustomException;
using Common.Validation.Api.Handle;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Validation.Api.ErrorHandle;

/// <summary>
/// Основной и центральный Middleware по управлению ошибками.
/// </summary>
public sealed class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IServiceScopeFactory _scopeFactory;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="next"><see cref="RequestDelegate"/>.</param>
    /// <param name="scopeFactory"><see cref="IServiceScopeFactory"/>.</param>
    public ErrorHandlingMiddleware(RequestDelegate next, IServiceScopeFactory scopeFactory)
    {
        _next = next;
        _scopeFactory = scopeFactory;
    }

   
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (LibraryException ex)
        {
            using var scope = _scopeFactory.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            await mediator.Send(new HandleLibraryExceptionContext
            {
                Exception = ex,
                HttpContext = context
            });
        }
        catch (ValidationException ex)
        {
            using var scope = _scopeFactory.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            await mediator.Send(new HandleValidationExceptionContext
            {
                Exception = ex,
                HttpContext = context
            });
        }
        /*catch (UnauthorizedAccessException ex)
        {
            using var scope = _scopeFactory.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            await mediator.Send(new HandleUnauthorizedExceptionContext
            {
                Exception = ex,
                HttpContext = context
            });
        }*/
    }
}