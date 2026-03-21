using Marten;

namespace BindingChaos.Stigmergy.Infrastructure.Persistence;

/// <summary>
/// Configuration for Marten in the Stigmergy bounded context.
/// </summary>
public static class StigmergyMartenConfiguration
{
    private const string StigmergySchemaName = "stigmergy";

    /// <summary>
    /// Configures Marten for the Stigmergy bounded context.
    /// </summary>
    /// <param name="options">The Marten store options to configure.</param>
    public static void Configure(StoreOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
    }
}
