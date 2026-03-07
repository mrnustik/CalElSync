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
            var delay = GetDelayUntilNextOccurrence();
            _logger.LogInformation("Next sync scheduled in {Delay}", delay);
            await Task.Delay(delay, stoppingToken);

            if (stoppingToken.IsCancellationRequested)
                break;

            _logger.LogInformation("Starting sync at {Time}", _timeProvider.GetUtcNow());
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var sync = scope.ServiceProvider.GetRequiredService<ISynchronizeCalendarEventsToTasks>();
                await sync.RunSynchronizationAsync(
                    new DateTimeInterval(DateTime.Today, DateTime.Today.AddDays(7)),
                    stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during sync");
            }
        }
    }

    internal TimeSpan GetDelayUntilNextOccurrence()
    {
        var cron = CronExpression.Parse(_scheduleOptions.Value.CronExpression);
        var next = cron.GetNextOccurrence(_timeProvider.GetUtcNow(), TimeZoneInfo.Utc)
            ?? throw new InvalidOperationException("Cron expression has no future occurrences.");
        return next - _timeProvider.GetUtcNow();
    }
}
