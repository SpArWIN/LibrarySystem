using Common.Validation.Api.Errors;
using Core.Application.Auth.Requests;
using FluentValidation;

namespace Core.Application.Auth.Validators;

/// <summary>
/// Валидатор регистрации.
/// </summary>
public sealed class RegisterUserRequestValidator : AbstractValidator<RegisterUserRequest>
{
    /// <summary>
    /// Правила валидации полей регистрации.
    /// </summary>
    public RegisterUserRequestValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .WithErrorCode(ApiErrors.Auth.UserError.UsernameRequired)
            .MinimumLength(3)
            .WithErrorCode(ApiErrors.Auth.UserError.UsernameTooShort);

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithErrorCode(ApiErrors.Auth.UserError.PasswordRequired)
            .MinimumLength(6)
            .WithErrorCode(ApiErrors.Auth.UserError.PasswordTooShort);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithErrorCode(ApiErrors.Auth.UserError.LastNameRequired);

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithErrorCode(ApiErrors.Auth.UserError.FirstNameRequired);
    }
}
