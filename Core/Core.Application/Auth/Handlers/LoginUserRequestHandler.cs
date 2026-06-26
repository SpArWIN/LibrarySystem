using Common.Contracts.Auth;
using Common.Validation.Api.CustomException;
using Common.Validation.Api.Errors;
using Core.Application.Auth.Mapping;
using Core.Application.Auth.Requests;
using MediatR;

namespace Core.Application.Auth.Handlers;

/// <summary>
/// Сборка <see cref="AuthorizeResponse"/> после прохождения пайплайна логина.
/// </summary>
public sealed class LoginUserRequestHandler : IRequestHandler<LoginUserRequest, AuthorizeResponse>
{
    /// <inheritdoc />
    public Task<AuthorizeResponse> Handle(LoginUserRequest request, CancellationToken cancellationToken)
    {
        var user = request.User
                   ?? throw LibraryException.Create(ApiErrors.Auth.UserNotFound);

        return Task.FromResult(new AuthorizeResponse
        {
            AccessToken = request.AccessToken,
            AccessExpiresAtUtc = request.AccessExpiresAtUtc,
            RefreshToken = request.RefreshToken,
            RefreshExpiresAtUtc = request.RefreshExpiresAtUtc,
            UserInfo = UserInfoMapper.ToUserInfoDto(user, request.Roles, request.Permissions),
        });
    }
}
