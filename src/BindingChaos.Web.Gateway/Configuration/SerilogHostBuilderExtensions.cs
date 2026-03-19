using Serilog;

namespace BindingChaos.Web.Gateway.Configuration;

/// <summary>
/// HostBuilder extension to configure Serilog from app configuration.
/// </summary>
public static class SerilogHostBuilderExtensions
{
    /// <summary>
    /// Wires Serilog using configuration and service-based enrichers.
    /// </summary>
    /// <param name="host">The host builder.</param>
    /// <returns>The host builder for chaining.</returns>
    public static IHostBuilder UseSerilogFromConfig(this IHostBuilder host)
    {
        host.UseSerilog((context, services, configuration) => configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext());
        return host;
    }
}
