using BindingChaos.SignalProcessing;
using BindingChaos.SignalProcessing.Configuration;
using BindingChaos.Stigmergy.Contracts;
using Npgsql;
using Wolverine;
using Wolverine.RabbitMQ;

var builder = Host.CreateApplicationBuilder(args);

var rabbitMqOptions = builder.Configuration.GetSection("RabbitMq").Get<RabbitMqOptions>() ?? new RabbitMqOptions();
var teiOptions = builder.Configuration.GetSection("Tei").Get<TeiOptions>() ?? new TeiOptions();
var clusteringOptions = builder.Configuration.GetSection("Clustering").Get<ClusteringOptions>() ?? new ClusteringOptions();
var connectionString = builder.Configuration.GetConnectionString("Database")
    ?? throw new InvalidOperationException("Database connection string not found");

var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
dataSourceBuilder.UseVector();
var dataSource = dataSourceBuilder.Build();

builder.Services.AddSingleton(dataSource);
builder.Services.AddHttpClient("tei", client => client.BaseAddress = new Uri(teiOptions.BaseUrl));
builder.Services.AddHttpClient("clustering", client => client.BaseAddress = new Uri(clusteringOptions.BaseUrl));
builder.Services.AddSingleton<ITeiClient, TeiClient>();
builder.Services.AddSingleton<IClusteringClient, ClusteringClient>();
builder.Services.AddSingleton<ISignalEmbeddingRepository, SignalEmbeddingRepository>();
builder.Services.AddHostedService<ClusteringJobScheduler>();

builder.UseWolverine(opts =>
{
    opts.UseRabbitMq(rabbit =>
    {
        rabbit.HostName = rabbitMqOptions.Host;
        rabbit.Port = rabbitMqOptions.Port;
        rabbit.UserName = rabbitMqOptions.Username;
        rabbit.Password = rabbitMqOptions.Password;
    }).AutoProvision();

    // Queue names must match ExternalIntegrationEventRoutingConvention.ToQueueName()
    // in CorePlatform.API: kebab-case of the full type name.
    opts.ListenToRabbitQueue("signal-captured-integration-event");

    opts.PublishMessage<ClustersIdentifiedIntegrationEvent>()
        .ToRabbitQueue("clusters-identified-integration-event");
});

var host = builder.Build();
await host.RunAsync().ConfigureAwait(false);
