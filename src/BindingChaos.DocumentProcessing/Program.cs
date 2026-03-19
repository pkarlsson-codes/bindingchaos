using Microsoft.Extensions.Hosting;
using Wolverine;
using Wolverine.RabbitMQ;
using Minio;

using BindingChaos.DocumentProcessing;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddMinio(configure => configure
    .WithEndpoint("localhost:9000")
    .WithCredentials("minioadmin", "minioadmin")
    .Build());

builder.UseWolverine(opts =>
{
    // Tell Wolverine to listen to the specific RabbitMQ queue MinIO hits
    opts.ListenToRabbitQueue("minio-attachments")
        .ProcessInline();

    // Optionally, send a 'ThumbnailCreated' message back to your Core API
    opts.PublishMessage<ThumbnailCreated>()
        .ToRabbitExchange("platform-events");
});

var host = builder.Build();
await host.RunAsync().ConfigureAwait(false);
