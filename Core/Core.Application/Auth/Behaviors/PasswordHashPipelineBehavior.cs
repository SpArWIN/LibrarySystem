using Core.Application.Auth.Context;
using Core.Application.Services.Hash;
using MediatR;

namespace Core.Application.Auth.Behaviors;

/// <summary>
/// Хеширование пароля перед сохранением пользователя.
/// </summary>
public sealed class PasswordHashPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequiresPasswordHashing
{
    private readonly IPasswordHasher _passwordHasher;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="passwordHasher"><see cref="IPasswordHasher"/>.</param>
    public PasswordHashPipelineBehavior(IPasswordHasher passwordHasher)
    {
        _passwordHasher = passwordHasher;
    }

    /// <inheritdoc />
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        request.Password = _passwordHasher.Hash(request.Password);
        return await next();
    }
}
