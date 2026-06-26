using Common.Localization.Services;
using Common.Validation.Api.Handle;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Common.Validation.Pipeline;

/// <summary>
/// Обработчик валидационных ошибок.
/// </summary>
public sealed class HandleValidationExceptionPipeline : IPipelineBehavior<HandleValidationExceptionContext, HandleErrorResponse>
{
    private readonly ILogger<HandleValidationExceptionPipeline> _logger;
    private readonly IErrorLocalization _localizationService;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="logger"><see cref="ILogger{TCategoryName}"/>.</param>
    /// <param name="localizationService"><see cref="IErrorLocalization"/>.</param>
    public HandleValidationExceptionPipeline(
        ILogger<HandleValidationExceptionPipeline> logger, 
        IErrorLocalization localizationService)
    {
        _logger = logger;
        _localizationService = localizationService;
    }


    /// <inheritdoc />
    public async Task<HandleErrorResponse> Handle(HandleValidationExceptionContext request, RequestHandlerDelegate<HandleErrorResponse> next, CancellationToken cancellationToken)
    {
        var context = request.HttpContext;
        var ex = request.Exception;
        _logger.LogWarning("Validation exception: {Errors}", 
            string.Join(", ", ex.Errors.Select(e => e.ErrorCode)));
        
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        context.Response.ContentType = "application/json";
        
        var errors = ex.Errors.Select(failRule => 
        {
            var parameters = failRule.FormattedMessagePlaceholderValues
                ?.ToDictionary(kv => kv.Key, kv => kv.Value);
            var message = parameters is not null && parameters.Count > 0
                ? _localizationService.GetString(failRule.ErrorCode, parameters)
                : _localizationService.GetString(failRule.ErrorCode);
            return new
            {
                key = failRule.ErrorCode,
                 message,
                field = failRule.PropertyName,
                attemptedValue = failRule.AttemptedValue?.ToString()
            };
        });
        var responseBody = new { errors };
        await context.Response.WriteAsJsonAsync(responseBody, cancellationToken);
        
        return new HandleErrorResponse
        {
            StatusCode = StatusCodes.Status400BadRequest,
            Response = responseBody
        };
    }
}