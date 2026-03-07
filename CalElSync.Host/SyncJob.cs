using CalElSync.Core.Common;
using CalElSync.Core.UseCases;
using Quartz;

namespace CalElSync.Host;

[DisallowConcurrentExecution]
public class SyncJob : IJob
{
    private readonly ISynchronizeCalendarEventsToTasks _sync;
    private readonly ILogger<SyncJob> _logger;

    public SyncJob(ISynchronizeCalendarEventsToTasks sync, ILogger<SyncJob> logger)
    {
        _sync = sync;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Starting sync at {Time}", DateTime.UtcNow);
        await _sync.RunSynchronizationAsync(
            new DateTimeInterval(DateTime.Today, DateTime.Today.AddDays(7)),
            context.CancellationToken);
    }
}
