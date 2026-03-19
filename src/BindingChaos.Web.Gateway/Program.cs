using BindingChaos.Web.Gateway.Configuration;
using BindingChaos.Web.Gateway.Configuration.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.ConfigureGatewayLogging();

try
{
    Log.Information(GatewayDefaults.Application.StartupMessage);

    builder.Services.AddGatewayServices(builder.Configuration);
    builder.Services.AddGatewayAuthentication(builder.Configuration);
    builder.Services.AddGatewayInfrastructure(builder.Configuration, builder.Environment);

    if (builder.Environment.IsDevelopment())
    {
        builder.Services.AddGatewayDevelopmentServices(builder.Configuration);
    }

    var app = builder.Build();

    app.ValidateStartupRequirements();
    app.ConfigureDevelopmentPipeline();
    app.ConfigureSecurityPipeline();
    app.ConfigureApiPipeline();

    Log.Information(GatewayDefaults.Application.StartupCompleteMessage);
    app.Run();
}
catch (Exception ex) when (ex is not OperationCanceledException)
{
    Log.Fatal(ex, GatewayDefaults.Application.UnexpectedTerminationMessage);
    throw;
}
finally
{
    Log.CloseAndFlush();
}
