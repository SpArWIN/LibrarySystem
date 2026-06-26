using Common.Localization.Services;
using Common.Validation.Api.Handle;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Common.Validation.Pipeline;

/// <summary>
/// Обработчик внутренних исключений Library System.
/// </summary>
public sealed class LibraryExceptionPipeline: IPipelineBehavior<HandleLibraryExceptionContext, HandleErrorResponse>
{
    private readonly IErrorLocalization _localizationService;
    private readonly ILogger<LibraryExceptionPipeline> _logger;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="localizationService"><see cref="ILocalizationService"/>.</param>
    /// <param name="logger"><see cref="ILogger"/>.</param>
    public LibraryExceptionPipeline(
        IErrorLocalization localizationService,
        ILogger<LibraryExceptionPipeline> logger)
    {
        _localizationService = localizationService;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<HandleErrorResponse> Handle(HandleLibraryExceptionContext request, RequestHandlerDelegate<HandleErrorResponse> next, CancellationToken cancellationToken)
    {
        var ex = request.Exception;
        var context = request.HttpContext;
        _logger.LogDebug(ex, "Business exception: {ErrorKey}", ex.ErrorKey);
        context.Response.StatusCode = (int)ex.Type;
        context.Response.ContentType = "application/json";
        var message = _localizationService.GetString(ex.ErrorKey, ex.Parameters);
        var responseBody = new
        {
            error = new
            {
                key = ex.ErrorKey,
                message,
                parameters = ex.Parameters.Count > 0 ? ex.Parameters : []
            }
        };
        await context.Response.WriteAsJsonAsync(responseBody, cancellationToken);
        return new HandleErrorResponse()
        {
            StatusCode = context.Response.StatusCode,
            Response = responseBody
        };
    }
}