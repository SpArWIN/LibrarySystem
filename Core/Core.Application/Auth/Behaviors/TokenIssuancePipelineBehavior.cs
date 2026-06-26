using Common.Contracts.Auth;
using Common.Extensions;
using Common.Validation.Api.CustomException;
using Common.Validation.Api.Errors;
using Core.Application.Auth.Context;
using Core.Application.Auth.Mapping;
using Core.Application.Auth.Requests;
using Core.Application.Services.TokenIssuer;
using MediatR;

namespace Core.Application.Auth.Behaviors;


/// <summary>
/// Выдача access/refresh после login/registration behaviors и handler.
/// Сначала <see cref="RequestHandlerDelegate{TResponse}"/> — пользователь уже в <see cref="IUserOperationContext.User"/>.
/// </summary>
public sealed class TokenIssuancePipelineBehavior<TRequest> : IPipelineBehavior<TRequest, AuthorizeResponse>
    where TRequest : class, IUserOperationContext
{
    private readonly IAuthTokenIssuer _tokenIssuer;
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="tokenIssuer"><see cref="IAuthTokenIssuer"/>.</param>
    public TokenIssuancePipelineBehavior(IAuthTokenIssuer tokenIssuer)
    {
        _tokenIssuer = tokenIssuer;
    }

    /// <inheritdoc />
    public async Task<AuthorizeResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<AuthorizeResponse> next,
        CancellationToken cancellationToken)
    {
        await next();
        
        var user = request.User
                   ?? throw LibraryException.Create(ApiErrors.Auth.UserNotFound);

        if (request.AccessToken.IsNullOrEmpty())
        {
            var submittedRefreshToken = request is LoginUserRequest login
                ? login.SubmittedRefreshToken
                : null;

            await _tokenIssuer.IssueForUserAsync(
                request,
                user,
                request.LibraryId,
                request.UnitOfWork,
                submittedRefreshToken,
                cancellationToken);
        }
        
        return new AuthorizeResponse
        {

            AccessToken = request.AccessToken,

            AccessExpiresAtUtc = request.AccessExpiresAtUtc,

            RefreshToken = request.RefreshToken,

            RefreshExpiresAtUtc = request.RefreshExpiresAtUtc,

            UserInfo = UserInfoMapper.ToUserInfoDto(user, request.Roles, request.Permissions),
        };
    }
}


