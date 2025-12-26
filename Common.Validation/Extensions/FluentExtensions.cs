using FluentValidation;
using FluentValidation.Results;

namespace Common.Validation.Extensions;

/// <summary>
/// Расширения на валидацию.
/// </summary>
public static class FluentExtensions
{
    /// <summary>
    /// Синхронно провалидировать значения.
    /// </summary>
    /// <param name="validator"><see cref="IValidator"/>.</param>
    /// <param name="instance">Тип.</param>
    /// <typeparam name="T">Тип ошибки.</typeparam>
    public static void ValidateThrow<T>(this IValidator<T> validator, T instance) =>
        validator.Validate(instance)
            .ThrowIfInvalid();
    
    /// <summary>
    /// Асинхронная валидация с выбрасыванием исключения.
    /// </summary>
    /// <param name="validator"><see cref="IValidator{T}"/>.</param>
    /// <param name="instance">Тип.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
    /// <typeparam name="T"></typeparam>
    public static async Task ValidateThrowAsync<T>(this IValidator<T> validator, T instance,
        CancellationToken cancellationToken = default) =>
        (await validator.ValidateAsync(instance, cancellationToken))
        .ThrowIfInvalid();
    
    /// <summary>
    /// Бросает кастомное исключение если результат валидации невалиден.
    /// </summary>
    private static void ThrowIfInvalid<TException>(this ValidationResult validationResult) 
        where TException : Exception, new()
    {
        if (!validationResult.IsValid)
        {
            throw new TException();
        }
    }
    
    /// <summary>
    /// Выбросить исключение в случае не валидности.
    /// </summary>
    private static void ThrowIfInvalid(this ValidationResult validationResult)
    {
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }
    }
    
    /// <summary>
    /// Пробросить ошибку в контекст валидации.
    /// </summary>
    /// <param name="context">Контекст валидации.</param>
    /// <param name="errorCode">Ошибка.</param>
    /// <param name="errorMessage">Сообщение ошибкию</param>
    /// <param name="customState">Дополнительное состояние.</param>
    /// <typeparam name="T"><see cref="T"/> Тип.</typeparam>
    public static void AddError<T>(
        this ValidationContext<T> context,
        string errorCode,
        string errorMessage,
        object? customState = null)
    {
        var message = context.MessageFormatter.BuildMessage(errorMessage);
        var failure = new ValidationFailure(context.PropertyPath, message)
        {
            ErrorCode = errorCode,
            CustomState = customState,
        };
        context.AddFailure(failure);
    }
}