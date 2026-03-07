using CalElSync.Core.Common;
using CalElSync.Core.UseCases;

namespace CalElSync.Host;

public class WeeklySyncService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<WeeklySyncService> _logger;

    public WeeklySyncService(IServiceScopeFactory scopeFactory, ILogger<WeeklySyncService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var delay = GetDelayUntilNextMonday0200Utc();
            _logger.LogInformation("Next sync scheduled in {Delay}", delay);
            await Task.Delay(delay, stoppingToken);

            if (stoppingToken.IsCancellationRequested)
                break;

            _logger.LogInformation("Starting weekly sync at {Time}", DateTime.UtcNow);
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
                _logger.LogError(ex, "An error occurred during the weekly sync");
            }
        }
    }

    private static TimeSpan GetDelayUntilNextMonday0200Utc()
    {
        var now = DateTime.UtcNow;
        var daysUntilMonday = ((int)DayOfWeek.Monday - (int)now.DayOfWeek + 7) % 7;
        if (daysUntilMonday == 0 && now.TimeOfDay >= TimeSpan.FromHours(2))
            daysUntilMonday = 7;
        var nextRun = now.Date.AddDays(daysUntilMonday).AddHours(2);
        return nextRun - now;
    }
}
