namespace Common.Messaging.Nats.Status;

/// <summary>
/// Статус сообщения.
/// </summary>
public enum MessageStatus
{
    /// <summary>
    /// Ожидает обработки.
    /// </summary>
    Pending = 0,
    
    /// <summary>
    /// В процессе обработки.
    /// </summary>
    Processing = 1,
    
    /// <summary>
    /// Успешно обработано.
    /// </summary>
    Completed = 2,
    
    /// <summary>
    /// Обработка завершилась ошибкой.
    /// </summary>
    Failed = 3,
    
    /// <summary>
    /// Обработка отменена.
    /// </summary>
    Cancelled = 4 ,
 
    /// <summary>
    /// Сообщение является дубликатом.
    /// </summary>
    Duplicate = 5
}