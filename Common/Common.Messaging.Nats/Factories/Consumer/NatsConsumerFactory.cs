using Common.Messaging.Nats.Clients;
using Common.Messaging.Nats.Contracts.Based;
using Common.Messaging.Nats.Settings;

namespace Common.Messaging.Nats.Factories.Consumer;

/// <inheritdoc />
public sealed class NatsConsumerFactory : INatsConsumerFactory
{
    private readonly INatsClient _natsClient;
    private readonly NatsJetStreamOptions _jetStreamOptions;

    public NatsConsumerFactory(INatsClient natsClient, NatsJetStreamOptions jetStreamOptions)
    {
        _natsClient = natsClient;
        _jetStreamOptions = jetStreamOptions;
    }

    /// <inheritdoc />
    public INatsConsumer<T> Create<T>() where T : ILibraryMessage
    {
       return new NatsConsumer<T>(_natsClient, _jetStreamOptions);
    }
}