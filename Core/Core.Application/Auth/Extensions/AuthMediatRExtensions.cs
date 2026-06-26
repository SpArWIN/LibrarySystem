using Common.Contracts.Auth;
using Core.Application.Auth.Behaviors;
using Core.Application.Auth.Requests;
using Core.Application.Services.TokenIssuer;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Application.Auth.Extensions;

/// <summary>
/// Регистрация auth-пайплайнов.
/// </summary>
public static class AuthMediatRExtensions
{
    /// <summary>
    /// Регистрирует <see cref="IAuthTokenIssuer"/> и auth pipeline behaviors.
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/>.</param>
    /// <returns><see cref="IServiceCollection"/>.</returns>
    /// <remarks>
    /// MediatR: первый зарегистрированный behavior — ближе к handler (выполняется позже).
    /// <see cref="TokenIssuancePipelineBehavior{TRequest}"/> регистрируется первым: после login/register вызывает <c>next()</c>, затем выдаёт токены.
    /// Регистрировать до ValidationPipelineBehavior, чтобы Validation шёл первым в цепочке.
    /// Register: Validation → PasswordHash → Registration → TokenIssuance → Handler.
    /// Login: Validation → Login → TokenIssuance → Handler.
    /// Refresh: Validation → RefreshToken → Handler.
    /// </remarks>
    public static IServiceCollection AddAuthMediatR(this IServiceCollection services)
    {
        services.AddScoped<IAuthTokenIssuer, AuthTokenIssuer>();

        services.AddTransient<IPipelineBehavior<RegisterUserRequest, AuthorizeResponse>,
            TokenIssuancePipelineBehavior<RegisterUserRequest>>();
        services.AddTransient<IPipelineBehavior<RegisterUserRequest, AuthorizeResponse>,
            RegistrationPipelineBehavior<RegisterUserRequest, AuthorizeResponse>>();
        services.AddTransient<IPipelineBehavior<RegisterUserRequest, AuthorizeResponse>,
            PasswordHashPipelineBehavior<RegisterUserRequest, AuthorizeResponse>>();

        services.AddTransient<IPipelineBehavior<LoginUserRequest, AuthorizeResponse>,
            TokenIssuancePipelineBehavior<LoginUserRequest>>();
        services.AddTransient<IPipelineBehavior<LoginUserRequest, AuthorizeResponse>,
            LoginPipelineBehavior<LoginUserRequest, AuthorizeResponse>>();

        services.AddTransient<IPipelineBehavior<RefreshTokensRequest, RefreshResponseDto>,
            RefreshTokenPipelineBehavior>();

        return services;
    }
}
