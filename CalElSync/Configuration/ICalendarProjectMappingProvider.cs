namespace CalElSync.Core.Configuration;

public interface ICalendarProjectMappingProvider
{
    Task<IReadOnlyCollection<CalendarProjectMapping>> GetCalendarMappingsAsync(CancellationToken ct);
}