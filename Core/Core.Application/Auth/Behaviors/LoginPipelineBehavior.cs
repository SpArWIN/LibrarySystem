using Common.Contracts.Auth;
using Common.Validation.Api.CustomException;
using Common.Validation.Api.Errors;
using Core.Application.Auth.Requests;
using Core.Application.Services.Hash;
using Core.Domain.Repository;
using Core.Infrastructure.Extensions.UnitOfWorks;
using MediatR;

namespace Core.Application.Auth.Behaviors;

/// <summary>
/// Проверка Bloom filter, учётных данных и загрузка пользователя.
/// </summary>
public sealed class LoginPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : LoginUserRequest
    where TResponse : AuthorizeResponse
{
    private readonly IUserPresenceCache _userPresenceCache;
    private readonly IPasswordHasher _passwordHasher;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="userPresenceCache"><see cref="IUserPresenceCache"/>.</param>
    /// <param name="passwordHasher"><see cref="IPasswordHasher"/>.</param>
    public LoginPipelineBehavior(IUserPresenceCache userPresenceCache, IPasswordHasher passwordHasher)
    {
        _userPresenceCache = userPresenceCache;
        _passwordHasher = passwordHasher;
    }

    /// <inheritdoc />
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!await _userPresenceCache.MightExistAsync(request.Username, cancellationToken))
        {
            throw LibraryException.Create(ApiErrors.Auth.InvalidCredentials);
        }

        var repository = request.UnitOfWork.GetAuthorizationRepository();
        var user = await repository.FindUserByUsernameAsync(request.Username, cancellationToken)
                   ?? throw LibraryException.Create(ApiErrors.Auth.InvalidCredentials);

        if (!_passwordHasher.Verify(request.Password, user.Password))
        {
            throw LibraryException.Create(ApiErrors.Auth.InvalidCredentials);
        }

        request.User = user;

        return await next();
    }
}
