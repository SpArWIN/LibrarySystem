using Common.Validation.Api.Errors;
using Core.Application.Auth.Requests;
using FluentValidation;

namespace Core.Application.Auth.Validators;

/// <summary>
/// Валидатор refresh-токена.
/// </summary>
public sealed class RefreshTokensRequestValidator : AbstractValidator<RefreshTokensRequest>
{
    /// <summary>
    /// Правила валидации refresh-запроса.
    /// </summary>
    public RefreshTokensRequestValidator()
    {
        RuleFor(x => x.SubmittedRefreshToken)
            .NotEmpty()
            .WithErrorCode(ApiErrors.Auth.InvalidToken);
    }
}
