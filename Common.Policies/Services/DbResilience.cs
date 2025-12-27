using Common.Policies.PipelineNames;
using Polly;
using Polly.Registry;

namespace Common.Policies.Services;

/// <inheritdoc />
public sealed class DbResilience(ResiliencePipelineProvider<string> provider) : IDbResilience
{
    public ResiliencePipeline Write => provider.GetPipeline(NamesPipeline.DbPipelines.Write);
    public ResiliencePipeline Read  => provider.GetPipeline(NamesPipeline.DbPipelines.Read);
}