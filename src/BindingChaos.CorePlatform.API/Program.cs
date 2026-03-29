using BindingChaos.CommunityDiscourse.Infrastructure;
using BindingChaos.CorePlatform.API.Infrastructure;
using BindingChaos.CorePlatform.API.Infrastructure.Extensions;
using BindingChaos.CorePlatform.API.Infrastructure.Seeding;
using BindingChaos.Societies.Infrastructure;
using BindingChaos.Stigmergy.Infrastructure;
using BindingChaos.Tagging.Infrastructure;
using Serilog;
using Wolverine;

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

    builder.Host.UseWolverine(opts =>
    {
        opts.Discovery.IncludeAssembly(typeof(CommunityDiscourseAssemblyReference).Assembly);
        opts.Discovery.IncludeAssembly(typeof(TaggingAssemblyReference).Assembly);
        opts.Discovery.IncludeAssembly(typeof(SocietiesAssemblyReference).Assembly);
        opts.Discovery.IncludeAssembly(typeof(StigmergyAssemblyReference).Assembly);
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
