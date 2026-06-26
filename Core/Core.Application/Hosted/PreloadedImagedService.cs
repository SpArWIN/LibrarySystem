using System.Threading.Channels;
using Common.Messaging.Nats.Contracts.Files;
using Common.Messaging.Nats.Factories.Consumer;
using Common.Messaging.Nats.Handlers;
using Common.Messaging.Nats.Messages;
using Common.Messaging.Nats.Subjects;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;


namespace Core.Application.Hosted;

/// <summary>
/// Подписка JetStream на <see cref="PreloadedImageAdded"/> и делегирование в <see cref="INatsMessageHandler{TMessage}"/>.
/// </summary>
public sealed class PreloadedImagedService : BackgroundService
{
    private readonly INatsConsumerFactory _consumerFactory;
    private readonly IServiceScopeFactory _scopeFactory;
    private static readonly ILogger Logger  = Log.ForContext<PreloadedImagedService>();

    public PreloadedImagedService(
        INatsConsumerFactory consumerFactory,
        IServiceScopeFactory scopeFactory)
    {
        _consumerFactory = consumerFactory;
        _scopeFactory = scopeFactory;
    }

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var observable = _consumerFactory.Create<PreloadedImageAdded>().FromDurableConsumer(
            NatsJetStreamIdentifiers.LibraryEventsStream,
            NatsJetStreamIdentifiers.CorePreloadedImageConsumer,
            NatsSubjects.FileMetaImagePreloadedAdded);

        var channel = Channel.CreateBounded<NatsMessage<PreloadedImageAdded>>(new BoundedChannelOptions(32)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = true,
            SingleWriter = false
        });

        using var subscription = observable.Subscribe(
            onNext: m => { channel.Writer.TryWrite(m); },
            onError: ex =>
            {
                Logger.Error(ex, "JetStream subscription error for {Subject}", NatsSubjects.FileMetaImagePreloadedAdded);
                channel.Writer.TryComplete(ex);
            },
            onCompleted: () => channel.Writer.TryComplete());

        try
        {
            await foreach (var msg in channel.Reader.ReadAllAsync(stoppingToken))
            {
                try
                {
                    await using var scope = _scopeFactory.CreateAsyncScope();
                    var handler = scope.ServiceProvider.GetRequiredService<INatsMessageHandler<PreloadedImageAdded>>();
                    await handler.HandleAsync(msg.Body, stoppingToken);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "Failed to handle PreloadedImageAdded for {ObjectKey}", msg.Body.ObjectKey);
                }
            }
        }
        finally
        {
            subscription.Dispose();
        }
    }
}
