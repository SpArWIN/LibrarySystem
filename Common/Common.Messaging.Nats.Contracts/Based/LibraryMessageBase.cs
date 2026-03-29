using Common.Messaging.Nats.Contracts.Files;
using ProtoBuf;

namespace Common.Messaging.Nats.Contracts.Based;

/// <summary>
/// Базовое сообщение публикаций.
/// </summary>
[ProtoContract]
[ProtoInclude(100, typeof(PreloadedImageAdded))]
public abstract class LibraryMessageBase : ILibraryMessage;