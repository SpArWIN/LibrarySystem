using Common.Messaging.Nats.Clients;
using Common.Messaging.Nats.Contracts.Based;
using Common.Messaging.Nats.Settings;

namespace Common.Messaging.Nats.Factories.Producer;

/// <inheritdoc />
public sealed class NatsProducer<TMessage> : INatsProducer<TMessage>
where TMessage : ILibraryMessage
{
    private readonly INatsClient _natsClient;
    private readonly NatsJetStreamOptions _jetStreamOptions;

    public NatsProducer(INatsClient natsClient, NatsJetStreamOptions jetStreamOptions)
    {
        _natsClient = natsClient;
        _jetStreamOptions = jetStreamOptions;
    }
    
    /// <inheritdoc />
    public async Task PublishAsync(
        string subject,
        TMessage body, 
        string? tenantId = null, 
        string? correlationId = null,
        CancellationToken ct = default)
    {
        _ = tenantId;
        _ = correlationId;
        await _natsClient.PublishAsync(subject, body, _jetStreamOptions.DefaultPubOptions, ct);
    }
}
