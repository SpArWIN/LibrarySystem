using Common.Extensions;
using Common.Messaging.Nats.Settings;
using Common.Messaging.Nats.SubscribeMode;
using Common.Policies.Pollicies.Nats;
using NATS.Client.JetStream.Models;

namespace Common.Messaging.Nats.Helper;

/// <summary>
/// Преобразователь Nats.
/// </summary>
public class NatsAdapter
{
    /// <summary>
    /// Преобразовать в ConsumerConfig из настроек.
    /// </summary>
    /// <param name="options"><see cref="NatsJsSubOptions"/>.</param>
    /// <returns></returns>
    public static ConsumerConfig ConvertToConsumerConfig(NatsJsSubOptions options)
    {
        var config = new ConsumerConfig();

        if (options.Durable.IsNotNullOrEmpty())
        {
            config.DurableName = options.Durable;
        }

        if (options.DeliverGroup.IsNotNullOrEmpty())
        {
            config.DeliverGroup = options.DeliverGroup;
        }

        if (options.Mode == NatsJsSubscribeMode.Push)
        {
            config.DeliverSubject = options.DeliverGroup ?? $"inbox.{Guid.NewGuid()}";
        }
        
        config.ReplayPolicy = options.ReplyPolicy switch
        {
            NatsJsReplayPolicy.Instant => ConsumerConfigReplayPolicy.Instant,
            NatsJsReplayPolicy.Original => ConsumerConfigReplayPolicy.Original,
            _ => ConsumerConfigReplayPolicy.Instant 
        };
        
        config.DeliverPolicy = options.DeliverPolicy switch
        {
            NatsJsDeliverPolicy.All => ConsumerConfigDeliverPolicy.All,
            NatsJsDeliverPolicy.Last => ConsumerConfigDeliverPolicy.Last,
            NatsJsDeliverPolicy.New => ConsumerConfigDeliverPolicy.New,
            NatsJsDeliverPolicy.ByStartSequence => ConsumerConfigDeliverPolicy.ByStartSequence,
            NatsJsDeliverPolicy.ByStartTime => ConsumerConfigDeliverPolicy.ByStartTime,
            NatsJsDeliverPolicy.LastPerSubject => ConsumerConfigDeliverPolicy.LastPerSubject,
            _ => ConsumerConfigDeliverPolicy.All
        };
        
        if (options.StartSequence > 0)
        {
            config.OptStartSeq = options.StartSequence;
            config.DeliverPolicy = ConsumerConfigDeliverPolicy.ByStartSequence;
        }
        
        if (options.StartTime != DateTimeOffset.MinValue)
        {
            config.OptStartTime = options.StartTime;
            
            if (options.StartSequence == 0)
            {
                config.DeliverPolicy = ConsumerConfigDeliverPolicy.ByStartTime;
            }
        }
        config.Description = $"Consumer for {options.Durable ?? "unknown"}";
        config.MaxDeliver = options.MaxDeliver;
        config.AckPolicy = ConsumerConfigAckPolicy.Explicit;
        config.AckWait = TimeSpan.FromMilliseconds(options.AckWait);
        
        return config;
    }
    

}