namespace Common.Validation.ErrorTypes;

/// <summary>
/// Типы ошибок соответствующие Http статус кодам.
/// </summary>
public enum ErrorType
{
    /// <summary>
    ///  Ошибка валидации, неверный формат
    /// </summary>
    BadRequest = 400,       
    
    /// <summary>
    /// Не аутентифицирован.
    /// </summary>
    Unauthorized = 401,   
    
    /// <summary>
    ///  Нет прав доступа.
    /// </summary>
    Forbidden = 403,        
    
    /// <summary>
    /// Ресурс не найден.
    /// </summary>
    NotFound = 404,        
    
    /// <summary>
    ///  Конфликт (уже существует).
    /// </summary>
    Conflict = 409,         
    
    /// <summary>
    /// Бизнес-правило нарушено.
    /// </summary>
    UnprocessableEntity = 422, 
    
    /// <summary>
    /// Слишком много запросов.
    /// </summary>
    TooManyRequests = 429,   
    
    /// <summary>
    ///  Внутренняя ошибка сервера
    /// </summary>
    InternalServerError = 500      
}