namespace CalElSync.Core.Configuration;

public class HardCodedCalendarMappingProvider : ICalendarMappingProvider
{
    public Task<IReadOnlyCollection<CalendarProjectMapping>> GetCalendarMappingsAsync(CancellationToken ct)
    {
        var hardcodedMappings = new List<CalendarProjectMapping>
        {
            new(
                new Uri(
                    "https://outlook.live.com/owa/calendar/30629dc5-f990-4e95-b0d0-e2f4e8197200/c30910cc-b9da-4c8b-868d-d3355484def2/cid-E92F019F441515BD/calendar.ics"),
                "2272739934")
        };
        return Task.FromResult<IReadOnlyCollection<CalendarProjectMapping>>(
            hardcodedMappings.AsReadOnly());
    }
}