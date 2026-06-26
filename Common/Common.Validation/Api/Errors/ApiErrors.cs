using Common.Validation.Attribute;
using Common.Validation.ErrorTypes;

namespace Common.Validation.Api.Errors;

/// <summary>
/// Внутренние ошибки Api.
/// </summary>
public static class ApiErrors
{
    private const string Root = "Api";
    
    
    /// <summary>
    /// Класс ошибок на работу с книгами.
    /// </summary>
    public static class Books
    {
        private const string Prefix = $"{Root}.Books.Errors";

        /// <summary>
        /// Книга не найдена.
        /// </summary>
        [ErrorCode(ErrorType.NotFound)]
        public static readonly string NotFound = $"{Prefix}.NotFound";
    }

    /// <summary>
    /// Ошибки связанные с аутентификацией или авторизацией пользователя.
    /// </summary>
    public static class Auth
    {
        private const string Prefix = $"{Root}.Auth.Errors";

        /// <summary>
        /// Неверный логин или пароль.
        /// </summary>
        [ErrorCode(ErrorType.Unauthorized)] 
        public const string InvalidCredentials = $"{Prefix}.InvalidCredentials";

        /// <summary>
        /// Срок действия токена истек.
        /// </summary>
        [ErrorCode(ErrorType.Unauthorized)] 
        public const string TokenExpired = $"{Prefix}.TokenExpired";

        /// <summary>
        /// Токен недействителен (неверная подпись, испорчен).
        /// </summary>
        [ErrorCode(ErrorType.Unauthorized)] 
        public const string InvalidToken = $"{Prefix}.InvalidToken";

        /// <summary>
        /// Refresh токен не найден или уже был использован.
        /// </summary>
        [ErrorCode(ErrorType.Unauthorized)]
        public static readonly string RefreshTokenNotFound = $"{Prefix}.RefreshTokenNotFound";

        /// <summary>
        /// Требуется аутентификация (токен отсутствует).
        /// </summary>
        [ErrorCode(ErrorType.Unauthorized)] 
        public const string Unauthorized = $"{Prefix}.Unauthorized";

        /// <summary>
        /// Нет прав для выполнения операции (роль не подходит).
        /// </summary>
        [ErrorCode(ErrorType.Forbidden)]
        public static readonly string Forbidden = $"{Prefix}.Forbidden";
        
        /// <summary>
        /// Недостаточно прав (scope не хватает).
        /// </summary>
        [ErrorCode(ErrorType.Forbidden)]
        public static readonly string InsufficientPermissions = $"{Prefix}.InsufficientPermissions";

        /// <summary>
        /// Пользователь не найден.
        /// </summary>
        [ErrorCode(ErrorType.NotFound)] 
        public const string UserNotFound = $"{Prefix}.UserNotFound";

        /// <summary>
        /// Пользовательские ошибки.
        /// </summary>
        public static class UserError
        {
            private const string UserPrefix = $"{Root}.Auth.Errors.UserErrors";

            /// <summary>
            /// Имя пользователя обязательно.
            /// </summary>
            [ErrorCode(ErrorType.UnprocessableEntity)]
            public const string UsernameRequired = $"{UserPrefix}.UserNameRequired";

            /// <summary>
            /// Имя пользователя слишком короткое.
            /// </summary>
            [ErrorCode(ErrorType.UnprocessableEntity)]
            public const string UsernameTooShort = $"{UserPrefix}.UsernameTooShort";

            /// <summary>
            /// Пароль обязателен.
            /// </summary>
            [ErrorCode(ErrorType.UnprocessableEntity)]
            public const string PasswordRequired = $"{UserPrefix}.PasswordRequired";

            /// <summary>
            /// Пароль слишком короткий.
            /// </summary>
            [ErrorCode(ErrorType.UnprocessableEntity)]
            public const string PasswordTooShort = $"{UserPrefix}.PasswordTooShort";

            /// <summary>
            /// Фамилия обязательна.
            /// </summary>
            [ErrorCode(ErrorType.UnprocessableEntity)]
            public const string LastNameRequired = $"{UserPrefix}.LastNameRequired";

            /// <summary>
            /// Имя обязательно.
            /// </summary>
            [ErrorCode(ErrorType.UnprocessableEntity)]
            public const string FirstNameRequired = $"{UserPrefix}.FirstNameRequired";
            
            /// <summary>
            /// Пользователь уже существует.
            /// </summary>
            [ErrorCode(ErrorType.Conflict)]
            public const string UserAlreadyExists = $"{UserPrefix}.UserAlreadyExists";
        }
    }
    
    
}