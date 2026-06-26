using Common.Extensions;
using Common.Http.Accessors;
using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace Common.Validation.Pipeline;

/// <summary>
/// Пайплайн для валидации.
/// </summary>
/// <typeparam name="TRequest">Тип запроса.</typeparam>
/// <typeparam name="TResponse">Тип ответа.</typeparam>
public sealed class ValidationPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;
    private readonly ICorrelationContextAccessor _contextAccessor;
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="validators">Все валидаторы.</param>
    /// <param name="contextAccessor"><see cref="ICorrelationContextAccessor"/>.</param>
    public ValidationPipelineBehavior(IEnumerable<IValidator<TRequest>> validators, 
        ICorrelationContextAccessor contextAccessor)
    {
        _validators = validators;
        _contextAccessor = contextAccessor;
    }

    /// <inheritdoc />
    public async Task<TResponse> Handle(TRequest request, 
        RequestHandlerDelegate<TResponse> next, 
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next();
        }
        var failures = new List<ValidationFailure>();
        
        await _validators.ForEachAsync(async (validator, _) =>
        {
            var result = await validator.ValidateAsync(request, cancellationToken);
            failures.AddRange(result.Errors);
        }, cancellationToken);
        if (failures.Any())
        {
            throw new ValidationException(failures);
        }

        if (_contextAccessor.CorrelationContext is not null)
        {
            _contextAccessor.CorrelationContext.Items[typeof(TRequest).Name] = request;
        }
        return await next();
    }
}