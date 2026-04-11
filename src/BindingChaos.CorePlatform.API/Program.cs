using BindingChaos.CommunityDiscourse.Infrastructure;
using BindingChaos.CorePlatform.API.Infrastructure;
using BindingChaos.CorePlatform.API.Infrastructure.Configuration;
using BindingChaos.CorePlatform.API.Infrastructure.Extensions;
using BindingChaos.CorePlatform.API.Infrastructure.Messaging;
using BindingChaos.CorePlatform.API.Infrastructure.Seeding;
using BindingChaos.IdentityProfile.Infrastructure;
using BindingChaos.Reputation.Infrastructure;
using BindingChaos.Societies.Infrastructure;
using BindingChaos.Stigmergy.Contracts;
using BindingChaos.Stigmergy.Infrastructure;
using BindingChaos.Tagging.Infrastructure;
using Serilog;
using Wolverine;
using Wolverine.RabbitMQ;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

try
{
    Log.Information("Starting BindingChaos Core Platform API");

    builder.Services.AddConfiguredControllers();
    builder.Services.AddConfiguredSwagger();
    builder.Services.AddConfiguredCors(builder.Configuration);
    builder.Services.AddCorePlatformServices(builder.Configuration, builder.Environment);

    var rabbitMqOptions = builder.Configuration.GetSection("RabbitMq").Get<RabbitMqOptions>() ?? new RabbitMqOptions();
    var useRabbitMq = !builder.Environment.IsEnvironment("Testing");

    builder.Host.UseWolverine(opts =>
    {
        if (useRabbitMq)
        {
            opts.UseRabbitMq(rabbit =>
            {
                rabbit.HostName = rabbitMqOptions.Host;
                rabbit.Port = rabbitMqOptions.Port;
                rabbit.UserName = rabbitMqOptions.Username;
                rabbit.Password = rabbitMqOptions.Password;
            }).AutoProvision();

            opts.RouteWith(new ExternalIntegrationEventRoutingConvention());
            opts.Policies.ConventionalLocalRoutingIsAdditive();

            opts.ListenToRabbitQueue(
                ExternalIntegrationEventRoutingConvention.ToQueueName(typeof(ClustersIdentifiedIntegrationEvent)));
        }

        opts.Discovery.IncludeAssembly(typeof(CommunityDiscourseAssemblyReference).Assembly);
        opts.Discovery.IncludeAssembly(typeof(TaggingAssemblyReference).Assembly);
        opts.Discovery.IncludeAssembly(typeof(SocietiesAssemblyReference).Assembly);
        opts.Discovery.IncludeAssembly(typeof(StigmergyAssemblyReference).Assembly);
        opts.Discovery.IncludeAssembly(typeof(IdentityProfileAssemblyReference).Assembly);
        opts.Discovery.IncludeAssembly(typeof(ReputationAssemblyReference).Assembly);
    });

    var app = builder.Build();

    app.ConfigureDevelopmentPipeline();
    app.ConfigureStandardPipeline();
    app.ConfigureEndpoints();

    await app.StartAsync().ConfigureAwait(false);

    if (app.Environment.IsDevelopment())
    {
        await DevelopmentSeeder.SeedAsync(app.Services).ConfigureAwait(false);
    }

    await app.WaitForShutdownAsync().ConfigureAwait(false);
}
catch (Exception ex) when (ex is not OperationCanceledException)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
    throw;
}
finally
{
    Log.CloseAndFlush();
}

/// <summary>Entry point — exposed as partial to allow WebApplicationFactory access in integration tests.</summary>
public partial class Program { }
