using CalElSync.Core.Common;
using CalElSync.Core.UseCases;

namespace CalElSync.Host;

public class SyncJobService
{
    private readonly ISynchronizeCalendarEventsToTasks _sync;
    private readonly ILogger<SyncJobService> _logger;

    public SyncJobService(ISynchronizeCalendarEventsToTasks sync, ILogger<SyncJobService> logger)
    {
        _sync = sync;
        _logger = logger;
    }

    public async Task RunAsync()
    {
        _logger.LogInformation("Starting sync at {Time}", DateTime.UtcNow);
        await _sync.RunSynchronizationAsync(
            new DateTimeInterval(DateTime.Today, DateTime.Today.AddDays(7)),
            CancellationToken.None);
    }
}
