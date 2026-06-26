using Common.Validation.Api.Errors;
using Core.Application.Auth.Requests;
using FluentValidation;

namespace Core.Application.Auth.Validators;

/// <summary>
/// Валидатор входа.
/// </summary>
public sealed class LoginUserRequestValidator : AbstractValidator<LoginUserRequest>
{
    /// <summary>
    /// Правила валидации полей входа.
    /// </summary>
    public LoginUserRequestValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .WithErrorCode(ApiErrors.Auth.UserError.UsernameRequired);

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithErrorCode(ApiErrors.Auth.UserError.PasswordRequired);
    }
}
