using CalElSync.Core.Configuration;

namespace CalElSync.Tests.Mocks.Configuration;

public class InMemoryCalendarMappingProvider : ICalendarMappingProvider
{
    private readonly ICollection<CalendarProjectMapping> _mappings = new List<CalendarProjectMapping>();

    public Task<IReadOnlyCollection<CalendarProjectMapping>> GetCalendarMappingsAsync(CancellationToken ct)
    {
        var result = _mappings
            .ToArray()
            .AsReadOnly();
        return Task.FromResult<IReadOnlyCollection<CalendarProjectMapping>>(result);
    }

    public void AddMapping(CalendarProjectMapping mapping)
    {
        _mappings.Add(mapping);
    }
}