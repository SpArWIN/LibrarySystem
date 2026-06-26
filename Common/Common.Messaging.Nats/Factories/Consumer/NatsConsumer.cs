using Common.Messaging.Nats.Clients;
using Common.Messaging.Nats.Contracts.Based;
using Common.Messaging.Nats.Messages;
using Common.Messaging.Nats.Settings;
using Serilog;

namespace Common.Messaging.Nats.Factories.Consumer;

/// <inheritdoc />
internal sealed class NatsConsumer<TMessage> : INatsConsumer<TMessage>
where TMessage : ILibraryMessage
{
    private static readonly ILogger Logger = Log.Logger.ForContext<NatsConsumer<TMessage>>();
    
    private readonly INatsClient _natsClient;
    private readonly NatsJetStreamOptions _jetStreamOptions;

    public NatsConsumer(INatsClient natsClient, NatsJetStreamOptions jetStreamOptions)
    {
        _natsClient = natsClient;
        _jetStreamOptions = jetStreamOptions;
    }

    /// <inheritdoc />
    public IObservable<NatsMessage<TMessage>> FromDurableConsumer(
        string streamName,
        string durableConsumer, string subjectFilter)
    {
       return _natsClient.FromJetStream<TMessage>(
           subjectFilter:subjectFilter,
           streamName:streamName,
           durableConsumer:durableConsumer,
           pubOpts:_jetStreamOptions.DefaultPubOptions,
           cancellationToken:CancellationToken.None
           );
    }

    /// <inheritdoc />
    public IObservable<NatsMessage<TMessage>> FromSubject(string subject, string? queueGroup = null)
    {
       return _natsClient.Observe<TMessage>(subject, queueGroup: queueGroup);
    }
    

}