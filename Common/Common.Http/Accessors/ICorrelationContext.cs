namespace Common.Http.Accessors;

/// <summary>
/// Контекст корреляции запросов.
/// Для того, что бы можно было отслеживать запросы.
/// </summary>
public interface ICorrelationContext
{
    /// <summary>
    /// Идентификатор запроса.
    /// </summary>
    public Guid CorrelationId { get; }
}