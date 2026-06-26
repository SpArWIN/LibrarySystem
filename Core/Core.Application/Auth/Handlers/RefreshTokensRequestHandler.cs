using Common.Contracts.Auth;
using Core.Application.Auth.Requests;
using MediatR;

namespace Core.Application.Auth.Handlers;

/// <summary>
/// Сборка <see cref="RefreshResponseDto"/> после прохождения пайплайна refresh.
/// </summary>
public sealed class RefreshTokensRequestHandler : IRequestHandler<RefreshTokensRequest, RefreshResponseDto>
{
    /// <inheritdoc />
    public Task<RefreshResponseDto> Handle(RefreshTokensRequest request, CancellationToken cancellationToken) =>
        Task.FromResult(new RefreshResponseDto
        {
            AccessToken = request.AccessToken,
            AccessExpiresAtUtc = request.AccessExpiresAtUtc,
            RefreshToken = request.RefreshToken,
            RefreshExpiresAtUtc = request.RefreshExpiresAtUtc,
        });
}
