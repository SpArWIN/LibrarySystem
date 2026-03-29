namespace Common.Messaging.Nats.Subjects;

/// <summary>
/// Имена JetStream stream и durable consumer для согласованности между сервисами.
/// </summary>
public static class NatsJetStreamIdentifiers
{
    /// <summary>
    /// Stream для событий домена <c>events.*</c> (создаётся при первом durable consumer).
    /// </summary>
    public const string LibraryEventsStream = "library-events";

    /// <summary>
    /// Durable consumer Core для <see cref="NatsSubjects.FileMetaImagePreloadedAdded"/>.
    /// </summary>
    public const string CorePreloadedImageConsumer = "core-preloaded-image";
}
