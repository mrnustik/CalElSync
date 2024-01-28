using CalElSync.Core.Common;

namespace CalElSync.Core.UseCases;

public interface ISynchronizeCalendarEventsToTasks
{
    Task RunSynchronizationAsync(DateTimeInterval interval, CancellationToken ct);
}