using Wolverine;

namespace BindingChaos.SignalProcessing;

/// <summary>
/// Fires <see cref="RunClusteringJob"/> on a fixed interval via Wolverine's local bus.
/// </summary>
public sealed partial class ClusteringJobScheduler(IServiceScopeFactory scopeFactory, ILogger<ClusteringJobScheduler> logger) : BackgroundService
{
    private static readonly TimeSpan Interval = TimeSpan.FromMinutes(15);

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(Interval);

        while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken).ConfigureAwait(false))
        {
            try
            {
                var scope = scopeFactory.CreateAsyncScope();
                await using (scope.ConfigureAwait(false))
                {
                    var bus = scope.ServiceProvider.GetRequiredService<IMessageBus>();
                    await bus.InvokeAsync(new RunClusteringJob(), stoppingToken).ConfigureAwait(false);
                }
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                LogClusteringJobFailed(logger, ex);
            }
        }
    }

    [LoggerMessage(Level = LogLevel.Error, Message = "Clustering job failed")]
    private static partial void LogClusteringJobFailed(ILogger logger, Exception ex);
}
