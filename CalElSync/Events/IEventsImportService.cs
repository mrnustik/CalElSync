using CalElSync.Core.Common;

namespace CalElSync.Core.Events;

public interface IEventsImportService
{
    Task<IReadOnlyCollection<Event>> GetEventsFromCalendarAsync(
        Uri calendarUrl,
        DateTimeInterval dateTimeInterval, 
        CancellationToken ct);
}