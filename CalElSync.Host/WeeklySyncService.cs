using CalElSync.Core.Common;
using CalElSync.Core.UseCases;
using CalElSync.Host.Configuration;
using Cronos;
using Microsoft.Extensions.Options;

namespace CalElSync.Host;

public class WeeklySyncService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<WeeklySyncService> _logger;
    private readonly IOptions<SyncScheduleOptions> _scheduleOptions;
    private readonly TimeProvider _timeProvider;

    public WeeklySyncService(
        IServiceScopeFactory scopeFactory,
        ILogger<WeeklySyncService> logger,
        IOptions<SyncScheduleOptions> scheduleOptions,
        TimeProvider timeProvider)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _scheduleOptions = scheduleOptions;
        _timeProvider = timeProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var nextOccurrence = GetNextOccurrence();
            var delay = nextOccurrence - _timeProvider.GetUtcNow();
            _logger.LogInformation("Next sync scheduled in {Delay}", delay);

            using var delayCts = new CancellationTokenSource(delay, _timeProvider);
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken, delayCts.Token);
            try
            {
                await Task.Delay(Timeout.Infinite, linkedCts.Token);
            }
            catch (OperationCanceledException) when (!stoppingToken.IsCancellationRequested)
            {
                // Delay expired — proceed with sync
            }

            if (stoppingToken.IsCancellationRequested)
                break;

            _logger.LogInformation("Starting sync at {Time}", _timeProvider.GetUtcNow());
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var sync = scope.ServiceProvider.GetRequiredService<ISynchronizeCalendarEventsToTasks>();
                var start = nextOccurrence.UtcDateTime.Date;
                await sync.RunSynchronizationAsync(
                    new DateTimeInterval(start, start.AddDays(7)),
                    stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during sync");
            }
        }
    }

    internal DateTimeOffset GetNextOccurrence()
    {
        var cron = CronExpression.Parse(_scheduleOptions.Value.CronExpression);
        return cron.GetNextOccurrence(_timeProvider.GetUtcNow(), TimeZoneInfo.Utc)
            ?? throw new InvalidOperationException("Cron expression has no future occurrences.");
    }
}
