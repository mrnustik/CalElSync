using CalElSync.Core.Common;

namespace CalElSync.Core;

public interface ISynchronizationHandler
{
    Task RunSynchronizationAsync(DateTimeInterval interval, CancellationToken ct);
}