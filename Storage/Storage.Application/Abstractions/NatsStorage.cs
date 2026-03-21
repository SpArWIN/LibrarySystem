using Common.Messaging.Nats.Clients;
using Common.Messaging.Nats.Contracts.Files;
using Common.Messaging.Nats.Subjects;
using Serilog;

namespace Storage.Application.Abstractions;

/// <inheritdoc />
public sealed class NatsStorage : INatsStorage
{
    private readonly INatsClient _natsClient;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="natsClient">.</param>
    public NatsStorage(INatsClient natsClient)
    {
        _natsClient = natsClient;
    }

    private static readonly ILogger Logger = Log.ForContext<NatsStorage>();
    /// <inheritdoc />
    public async Task PublishPreloadedImageAddedAsync(PreloadedImageAdded evt, CancellationToken ct = default)
    {
        try
        {
            
            await _natsClient.PublishAsync(
                NatsSubjects.FileMetaImagePreloadedAdded, evt, null, ct);
            Logger.Information("Published {EventType} → bucket: {Bucket}, key: {ObjectKey}",
                nameof(PreloadedImageAdded), 
                evt.Bucket, evt.ObjectKey);
        }
        catch (Exception e)
        {
            Logger.Error(e,
                "Failed to publish {EventType} for {ObjectKey}",
                nameof(PreloadedImageAdded), evt.ObjectKey);
        }
    }
}