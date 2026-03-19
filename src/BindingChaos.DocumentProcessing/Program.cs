using BindingChaos.DocumentProcessing;
using BindingChaos.DocumentProcessing.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Minio;
using Wolverine;
using Wolverine.RabbitMQ;

var builder = Host.CreateApplicationBuilder(args);

var minioOptions = builder.Configuration.GetSection("Minio").Get<MinioOptions>() ?? new MinioOptions();
var rabbitMqOptions = builder.Configuration.GetSection("RabbitMq").Get<RabbitMqOptions>() ?? new RabbitMqOptions();
var messagingOptions = builder.Configuration.GetSection("Messaging").Get<MessagingOptions>() ?? new MessagingOptions();

builder.Services.AddMinio(configure => configure
    .WithEndpoint(minioOptions.Endpoint)
    .WithCredentials(minioOptions.AccessKey, minioOptions.SecretKey)
    .WithSSL(minioOptions.UseSsl)
    .Build());

builder.UseWolverine(opts =>
{
    opts.UseRabbitMq(rabbit =>
    {
        rabbit.HostName = rabbitMqOptions.Host;
        rabbit.Port = rabbitMqOptions.Port;
        rabbit.UserName = rabbitMqOptions.Username;
        rabbit.Password = rabbitMqOptions.Password;
    });

    opts.ListenToRabbitQueue(messagingOptions.AttachmentsQueue)
        .ProcessInline();

    opts.PublishMessage<ThumbnailCreated>()
        .ToRabbitExchange(messagingOptions.EventsExchange);
});

var host = builder.Build();
await host.RunAsync().ConfigureAwait(false);
