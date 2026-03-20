namespace Common.Policies.Pollicies.Nats;

/// <summary>
/// Перечисление повторных попыток доставки.
/// </summary>
public enum NatsJsReplayPolicy
{
    /// <summary>
    /// Сообщения доставляются моментально, как только потребитель готов.
    /// <remarks>
    /// Быстрая доставка.
    /// </remarks>
    /// </summary>
    Instant = 0,
    
    /// <summary>
    /// Сообщения доставляются с оригинальными задержками между ними.
    /// <remarks>
    ///Сохраняет временные интервалы между сообщениями, как при первоначальной публикации.
    /// </remarks>
    /// </summary>
    Original = 1
}