using Common.Policies.PipelineNames;
using Polly;
using Polly.Registry;

namespace Common.Policies.Services;

/// <inheritdoc />
public sealed class DbResilience(ResiliencePipelineProvider<string> provider) : IDbResilience
{
    /// <inheritdoc />
    public ResiliencePipeline Write => provider.GetPipeline(NamesPipeline.DbPipelines.Write);
    
    /// <inheritdoc />
    public ResiliencePipeline Read  => provider.GetPipeline(NamesPipeline.DbPipelines.Read);
    
    /// <inheritdoc />
    public ResiliencePipeline PublishNats  => provider.GetPipeline(NamesPipeline.Nats.Publish);
    
    /// <inheritdoc />
    public ResiliencePipeline RequestNats  => provider.GetPipeline(NamesPipeline.Nats.Request);
    
    /// <inheritdoc />
    public ResiliencePipeline NatsConnect  => provider.GetPipeline(NamesPipeline.Nats.Connect);
    
    /// <inheritdoc />
    public ResiliencePipeline NatsJetStream  => provider.GetPipeline(NamesPipeline.Nats.JetStream);

    public ResiliencePipeline DatabaseConnect => provider.GetPipeline(NamesPipeline.DbPipelines.Connect);
}