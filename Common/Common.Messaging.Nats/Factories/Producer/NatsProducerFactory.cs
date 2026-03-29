using Common.Messaging.Nats.Clients;
using Common.Messaging.Nats.Contracts.Based;
using Common.Messaging.Nats.Factories.Consumer;
using Common.Messaging.Nats.Settings;

namespace Common.Messaging.Nats.Factories.Producer;

/// <inheritdoc />
public sealed class NatsProducerFactory : INatsProducerFactory
{
    private readonly INatsClient _natsClient;
    private readonly NatsJetStreamOptions _jetStreamOptions;

    public NatsProducerFactory(INatsClient natsClient, NatsJetStreamOptions jetStreamOptions)
    {
        _natsClient = natsClient;
        _jetStreamOptions = jetStreamOptions;
    }


    /// <inheritdoc />
    public INatsProducer<TMessage> Create<TMessage>()
        where TMessage : ILibraryMessage
    {
      return new NatsProducer<TMessage>(_natsClient, _jetStreamOptions);
    }
}