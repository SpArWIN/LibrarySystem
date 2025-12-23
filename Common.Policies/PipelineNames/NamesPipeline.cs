namespace Common.Policies.PipelineNames;

/// <summary>
/// Имена пайплайнов.
/// </summary>
public static class NamesPipeline
{
    public static class DbPipelanes
    {
        /// <summary>Пайплайн для write-операций БД (транзакции).</summary>
        public const string Write = nameof(Write);
        
        /// <summary>Пайплайн для read-операций БД (без транзакции).</summary>
        public const string Read = nameof(Read);
    }
    
    public static class Nats
    {
        /// <summary>Публикации.</summary>
        public const string Publish = nameof(Publish);

        /// <summary>Подписки.</summary>
        public const string Subscribe = nameof(Subscribe);
    }
    
    public static class Http
    {
        /// <summary>Исходящие HTTP-запросы.</summary>
        public const string Outbound = nameof(Outbound);
    }

    public static class Storage
    {
        /// <summary>Операции хранения/IO (MinIO, FS).</summary>
        public const string Io = nameof(Io);
    }
    
}