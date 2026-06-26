using Common.Contracts.Auth;
using Core.Application.Auth.Requests;
using Core.Application.Services.TokenIssuer;
using MediatR;

namespace Core.Application.Auth.Behaviors;

/// <summary>
/// Проверка refresh-сессии, ротация и выдача новой пары токенов.
/// </summary>
public sealed class RefreshTokenPipelineBehavior : IPipelineBehavior<RefreshTokensRequest, RefreshResponseDto>
{
    private readonly IAuthTokenIssuer _tokenIssuer;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="tokenIssuer"><see cref="IAuthTokenIssuer"/>.</param>
    public RefreshTokenPipelineBehavior(IAuthTokenIssuer tokenIssuer)
    {
        _tokenIssuer = tokenIssuer;
    }

    /// <inheritdoc />
    public async Task<RefreshResponseDto> Handle(
        RefreshTokensRequest request,
        RequestHandlerDelegate<RefreshResponseDto> next,
        CancellationToken cancellationToken)
    {
        await _tokenIssuer.RotateRefreshAsync(request, cancellationToken);
        return await next();
    }
}
