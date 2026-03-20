using ProtoBuf;

namespace Common.Messaging.Nats.Contracts.Based;

/// <summary>
/// Базовое сообщение публикаций.
/// </summary>
[ProtoContract]
public abstract class LibraryMessageBase : ILibraryMessage;