using System.Reactive.Disposables;
using System.Reactive.Linq;
using Common.Extensions;
using Common.Messaging.Nats.Authorize;
using Common.Messaging.Nats.Contracts.Based;
using Common.Messaging.Nats.Messages;
using Common.Messaging.Nats.Settings;
using Common.Policies.Pollicies.Nats;
using Common.Policies.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NATS.Client.Core;
using NATS.Client.JetStream;
using NATS.Client.JetStream.Models;
using NATS.Net;
using Serilog;

namespace Common.Messaging.Nats.Clients;

/// <inheritdoc />
public sealed class NatsClient : INatsClient
{
    private static readonly ILogger Logger = Log.ForContext<NatsClient>();
    private readonly INatsJSContext _jetStreamContext;
    private readonly NatsConnection? _connection;
    private readonly NatsConnectionOptions _options;
    private readonly NatsJetStreamOptions _jetStreamOptions;
    private readonly SemaphoreSlim _connectLock = new(1, 1);
    private readonly IServiceProvider _serviceProvider;
    private readonly IDbResilience _dbResilience;

    /// <summary>
    /// Клиент.
    /// </summary>
    /// <param name="options"><see cref="NatsConnectionOptions"/>.</param>
    /// <param name="jetStreamOptions"><see cref="NatsJetStreamOptions"/>.</param>
    /// <param name="dbResilience"><see cref="IDbResilience"/>.</param>
    /// <param name="serviceProvider"><see cref="IServiceProvider"/>.</param>
    public NatsClient(
        IOptions<NatsConnectionOptions>? options, 
        IOptions<NatsJetStreamOptions>? jetStreamOptions,
        IDbResilience dbResilience, 
        IServiceProvider serviceProvider 
        )
    {
        _options = options?.Value;
        _connection = CreateNatsConnection();
        _dbResilience = dbResilience;
        _serviceProvider = serviceProvider;
        _jetStreamContext = _connection?.CreateJetStreamContext()!;
        _jetStreamOptions = jetStreamOptions?.Value ?? new NatsJetStreamOptions();
        
    }
    
    /// <inheritdoc />
    public bool IsConnected => _connection?.ConnectionState == NatsConnectionState.Connecting;

    /// <inheritdoc />
    public async Task ConnectAsync(CancellationToken cancellationToken = default)
    {
        if (IsConnected)
        {
            return;
        }

        await _dbResilience.NatsConnect.ExecuteAsync(async token =>
        {
            await _connectLock.WaitAsync(token);
            try
            {
                if (IsConnected)
                {
                    return;
                }

                Logger.Information("Connecting to NATS server at {BrokerUrl}", _options.Broker);
                
                if (_connection is null)
                {
                    throw new InvalidOperationException("NATS connection instance is null");
                }
                
                try
                {
                    await _connection.PingAsync(token);
                    if (IsConnected)
                    {
                        Logger.Information("Already connected (ping succeeded)");
                        return;
                    }
                }
                catch (Exception pingEx)
                {
                    Logger.Debug(pingEx, "Initial ping failed — will try full connect");
                  
                }
                
                await _connection.ConnectAsync();
                
                if (!IsConnected)
                {
                    throw new InvalidOperationException("ConnectAsync completed, but IsConnected is still false");
                }

                Logger.Information("Successfully connected to NATS server at {BrokerUrl}", _options.Broker);
            }
            catch (OperationCanceledException)
            {
                Logger.Warning("NATS connection cancelled by token");
                throw;
            }
            catch (NatsJSConnectionException nex)
            {
                Logger.Error(nex, "NATS-specific error during connection to {BrokerUrl}", _options.Broker);
                throw;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Failed to connect to NATS server at {BrokerUrl}", _options.Broker);
                throw;
            }
            finally
            {
                _connectLock.Release();
            }
        }, cancellationToken);
    }


    /// <inheritdoc />
    public async Task PublishAsync<T>(string subject, 
        T message, 
        NatsJsPubOptions? opts = null,
        CancellationToken cancellationToken = default) where T : ILibraryMessage
    {
        await ConnectAsync(cancellationToken);
        await _dbResilience.PublishNats.ExecuteAsync(async _ =>
        {
            var options = CreateNatsJetStreamOptions(opts);
            var serializer = GetSerializer<T>();
            var ack = await _jetStreamContext.PublishAsync(subject, message, serializer, options, null, cancellationToken);
            ack.EnsureSuccess();
        }, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<TResponse> RequestAsync<TRequest, TResponse>(
        string subject, 
        TRequest request, 
        TimeSpan? timeout = null,
        NatsJsPubOptions? pubOpts = null, 
        CancellationToken cancellationToken = default) where TRequest : ILibraryMessage where TResponse : ILibraryMessage
    {
        await ConnectAsync(cancellationToken);
        var requestSerializer = GetSerializer<TRequest>();
        var responseSerializer = GetSerializer<TResponse>();
        var response = await _connection.RequestAsync(
            subject: subject,
            data: request,
            requestSerializer: requestSerializer,
            replySerializer: responseSerializer,
            replyOpts: new NatsSubOpts { Timeout = timeout },
            cancellationToken: cancellationToken);
        return response.Data;
    }

    /// <inheritdoc />
    public IObservable<NatsMessage<T>> FromJetStream<T>(
        string subjectFilter, 
        string streamName, 
        string durableConsumer,
        NatsJsPubOptions? pubOpts = null, 
        ConsumerConfig? consumerConfig = null,
        CancellationToken cancellationToken = default) where T : ILibraryMessage
    {
        return Observable.Create<NatsMessage<T>>(async (observer, ct) =>
        {
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            var token = linkedCts.Token;
            try
            {
                var consumer = await GetOrCreateConsumerAsync(
                    streamName,
                    durableConsumer,
                    subjectFilter,
                    consumerConfig,
                    token);
                var serializer = GetSerializer<T>();
                var consumerOptions = new NatsJSConsumeOpts()
                {
                    MaxMsgs = 100,
                    IdleHeartbeat = TimeSpan.FromSeconds(30)
                };

                await foreach (var message in consumer.ConsumeAsync(
                                   serializer: serializer,
                                   opts: consumerOptions,
                                   cancellationToken: token))
                {
                    if (token.IsCancellationRequested)
                    {
                        break;
                    }

                    try
                    {
                        var natsMessage = new NatsMessage<T>()
                        {
                            Body = message.Data,
                            Subject = message.Subject,
                            Reply = message.ReplyTo,
                            ClientId = _options.ClientId,
                        };
                        observer.OnNext(natsMessage);
                        await message.AckAsync(cancellationToken: token);

                    }
                    catch (Exception ex)
                    {
                        observer.OnError(new InvalidOperationException(
                            $"Failed to process message on subject {message.Subject}", ex));
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                observer.OnError(ex);
            }
            finally
            {
                observer.OnCompleted();
            }

            return Disposable.Empty;
        });
    }

    /// <inheritdoc />
    public IObservable<NatsMessage<T>> Observe<T>(
        string subject,
        string? queueGroup = null, 
        CancellationToken cancellationToken = default) 
        where T : ILibraryMessage
    {
        return Observable.Create<NatsMessage<T>>(async (observer, token) =>
        {
            var serializer = GetSerializer<T>();
            var subscription = _connection.SubscribeAsync(
                subject: subject,
                serializer: serializer,
                cancellationToken: token);
            try
            {
                await foreach (var message in subscription)
                {
                    var natsMessage = new NatsMessage<T>
                    {
                        Body = message.Data!,
                        Subject = message.Subject,
                        Reply = message.ReplyTo,
                        ClientId = _options.ClientId
                    };
                    observer.OnNext(natsMessage);
                }
            }
            catch (OperationCanceledException)
            {
                observer.OnCompleted();
            }
            catch (Exception ex)
            {
                observer.OnError(ex);
            }
        });
    }
    

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        try
        {
            if (_jetStreamContext is IAsyncDisposable jetStreamDisposable)
            {
                await jetStreamDisposable.DisposeAsync();
            }
            if (_connection is IAsyncDisposable connectionDisposable)
            {
                await connectionDisposable.DisposeAsync();
            }
            else if (_connection is IDisposable syncDisposable)
            {
                syncDisposable.Dispose();
            }
        }
        finally
        {
            // ReSharper disable once GCSuppressFinalizeForTypeWithoutDestructor
            GC.SuppressFinalize(this);
        }
    }

    private async Task<INatsJSConsumer> GetOrCreateConsumerAsync(
        string streamName,
        string durableConsumer,
        string filterSubject,
        ConsumerConfig? consumerConfig = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            
            return await _jetStreamContext.GetConsumerAsync(
                streamName,
                durableConsumer,
                cancellationToken);
            
        }
        catch (NatsJSException ex) when (IsConsumerNotFound(ex))
        {
            await EnsureStreamExistsAsync(streamName, filterSubject, cancellationToken);
            return await CreateOrUpdateConsumerAsync(streamName, durableConsumer, filterSubject, consumerConfig , cancellationToken);
        }
    }

    private async Task EnsureStreamExistsAsync(string streamName, string subjectPattern, CancellationToken ct = default)
    {
        var config = CreateStreamConfig(streamName, subjectPattern);
        try
        {
            await _jetStreamContext.GetStreamAsync(streamName, cancellationToken:ct);
        }
        catch (NatsJSException ex) when (IsStreamNotFound(ex))
        {
            await _jetStreamContext.CreateOrUpdateStreamAsync(config, ct);
        }
        catch (NatsJSException ex2) when (ex2.Message.Contains("overlap") || IsStreamNotFound(ex2))
        {
            throw new InvalidOperationException($"Stream/consumer issue: {ex2.Message}");
        }
    }
    
    
    private StreamConfig CreateStreamConfig(string streamName, string subjectPattern)
    {
        return new StreamConfig
        {
            Name = streamName,
            Subjects = GetStreamSubjects(subjectPattern),
            Storage = StreamConfigStorage.File,
            Retention = StreamConfigRetention.Limits,
            MaxMsgs = 100_000,
            MaxBytes = 100 * 1024 * 1024, // 100 MB
            MaxAge = TimeSpan.FromDays(30),
            MaxMsgSize = 1024 * 1024, // 1 MB
            DuplicateWindow = TimeSpan.FromMinutes(2),
            Discard = StreamConfigDiscard.Old,
            NumReplicas = 1,
            DenyDelete = false,
            DenyPurge = false,
            DiscardNewPerSubject = false,
            MaxConsumers = -1,
            MaxMsgsPerSubject = -1,
            Metadata = new Dictionary<string, string>
            {
                ["created-by"] = _options?.ClientId,
                ["created-at"] = DateTime.UtcNow.ToString("O")
            }
        };
    }

    private async Task<INatsJSConsumer> CreateOrUpdateConsumerAsync(
        string streamName,
        string durableName,
        string filterSubject,
        ConsumerConfig? consumerConfig = null,
        CancellationToken cancellationToken = default)
    {

        var config = consumerConfig ?? CreateConsumerConfig(durableName, filterSubject);
        try
        {
            await _jetStreamContext.DeleteConsumerAsync(
                streamName,
                durableName,
                cancellationToken);

            var existingConsumer = await _jetStreamContext.GetConsumerAsync(
                streamName,
                durableName,
                cancellationToken);

            var existingConfig = existingConsumer.Info.Config;

            var isConfigMatch =
                existingConfig.FilterSubject == config.FilterSubject &&
                existingConfig.DeliverSubject == config.DeliverSubject &&
                existingConfig.DeliverGroup == config.DeliverGroup &&
                existingConfig.DeliverPolicy == config.DeliverPolicy;

            if (isConfigMatch)
            {
                return existingConsumer;
            }
            
            return await _jetStreamContext.CreateOrUpdateConsumerAsync(
                streamName,
                config,
                cancellationToken);
        }
        catch (NatsJSException ex) when (IsConsumerNotFound(ex))
        {
            return await _jetStreamContext.CreateOrUpdateConsumerAsync(
                streamName,
                config,
                cancellationToken);
        }
    }

    private string[] GetStreamSubjects(string subjectPattern)
    {
        if (subjectPattern.EndsWith(".*"))
        {
            return new[] { subjectPattern.Replace(".*", ".>") };
        }
        return new[] { subjectPattern };
    }
    
    private ConsumerConfig CreateConsumerConfig(string durableName, string filterSubject)
    {
        return new ConsumerConfig
        {
            Name = durableName,
            DurableName = durableName,
            FilterSubject = filterSubject,
            DeliverSubject = GetDeliverSubject(durableName),
            DeliverGroup = $"{durableName}_group",

            DeliverPolicy = ConsumerConfigDeliverPolicy.New,
            ReplayPolicy = ConsumerConfigReplayPolicy.Instant,


            AckPolicy = ConsumerConfigAckPolicy.Explicit,
            AckWait = TimeSpan.FromSeconds(30),
            MaxAckPending = 1000,
            
            MaxDeliver = 5,
            RateLimitBps = 100,
            SampleFreq = "100%",
            
            FlowControl = false,
            IdleHeartbeat = TimeSpan.FromSeconds(30),
            
            HeadersOnly = false,
            
            MemStorage = false
        };
    }
    
    private static string GetDeliverSubject(string durableName)
    {
        return $"deliver.to.{durableName}";
    }

    private static NatsJSPubOpts CreateNatsJetStreamOptions(NatsJsPubOptions? options)
    {
        return new NatsJSPubOpts()
        {
            MsgId = options?.MsgId,
            ExpectedStream = options?.ExpectedStream,
            RetryAttempts = options?.MaxWait ?? 3,
            ExpectedLastMsgId = options?.ExpectedLastMsgId,
            ExpectedLastSequence = options?.ExpectedLastSequence,
            RetryWaitBetweenAttempts = options?.RetryInterval ?? TimeSpan.FromSeconds(1),
        };
    }

    private NatsConnection CreateNatsConnection()
    {
        var opts = BuildNatsOptions();
        
        if (_options.Auth is not null)
        {
            opts = ConfigureAuth(opts, _options.Auth);
        }
        
        return new NatsConnection(opts);
    }

    private NatsOpts BuildNatsOptions()
    {
        var opts = NatsOpts.Default with
        {
            Url = _options.Broker,
            Name = _options.ClientId ?? $"NatsClient-{Guid.NewGuid():N[..8]}",
            ConnectTimeout = TimeSpan.FromSeconds(_options.ConnectTimeout),
            ReconnectWaitMax = TimeSpan.FromSeconds(_options.ReconnectWait),
            ReconnectWaitMin = TimeSpan.FromSeconds(_options.ReconnectWait / 2),
            Echo = _options.Echo,
            Verbose = _options.Verbose,
            MaxPingOut = _options.MaxPingsOut,
            MaxReconnectRetry = _options.MaxReconnect,
            PingInterval = _options.PingInterval,
            RequestTimeout = TimeSpan.FromSeconds(_options.RequestTimeout),
        };

        return opts;
    }

    private INatsSerializer<T> GetSerializer<T>() where T : ILibraryMessage
    {
        return _serviceProvider.GetRequiredService<INatsSerializer<T>>();
    }
    
    private bool IsStreamNotFound(Exception ex)
    {
        return IsNotFoundError(ex, "stream");
    }

    private NatsJSConsumeOpts CreateDefaultConsumeOpts()
    {
        return new NatsJSConsumeOpts
        {
            MaxMsgs = _jetStreamOptions.MaxMessagesBufferSize,
            MaxBytes = _jetStreamOptions.MaxBytesBufferSize,
        };
    }
    
    private NatsJsReplayPolicy ConvertReplayPolicy(NatsJsReplayPolicy policy)
    {
        return policy switch
        {
            NatsJsReplayPolicy.Instant => NatsJsReplayPolicy.Instant,
            NatsJsReplayPolicy.Original => NatsJsReplayPolicy.Original,
            _ => NatsJsReplayPolicy.Instant
        };
    }
    
 
    private static NatsOpts ConfigureAuth(NatsOpts opts, NatsAuthOptions authOpts)
    {
        var auth = NatsAuthOpts.Default;
        if (authOpts.Jwt.IsNotNullOrEmpty() && auth.NKey.IsNotNullOrEmpty())
        {
            auth  = auth with {Jwt = authOpts.Token, NKey = authOpts.NKey};
        }

        else  if (authOpts.Username.IsNotNullOrEmpty() && authOpts.Password.IsNotNullOrEmpty())
        {
            auth = auth with {Username = authOpts.Username, Password = authOpts.Password};
        }

        else if (authOpts.Token.IsNotNullOrEmpty())
        {
            auth = auth with {Token = authOpts.Token};
        }
        else if (authOpts.CredsFile.IsNotNullOrEmpty())
        {
            auth = auth with{CredsFile = authOpts.CredsFile};
        }
        else if (authOpts.NKey.IsNotNullOrEmpty())
        {
            auth = auth with {NKey = authOpts.NKey};
        }

        return opts with { AuthOpts = auth };
    }
    
    private bool IsNotFoundError(Exception ex, string entityType)
    {
        if (ex is not NatsJSException jsEx) 
            return false;
            
        var message = jsEx.Message.ToLowerInvariant();
        return message.Contains("404") ||
               message.Contains("not found") ||
               message.Contains($"no such {entityType}") ||
               message.Contains($"{entityType} does not exist") ||
               message.Contains("10059"); 
    }
    
    private bool IsConsumerNotFound(Exception ex)
    {
        return IsNotFoundError(ex, "consumer");
    }
    
}