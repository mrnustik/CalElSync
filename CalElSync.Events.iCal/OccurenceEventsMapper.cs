using CalElSync.Core.Events;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;

namespace CalElSync.Events.iCal;

public static class OccurenceEventsMapper
{
    public static Event MapToEvent(Occurrence occurrence)
    {
        var calendarEvent = (CalendarEvent)occurrence.Source;
        return new Event(
            calendarEvent.Summary.Trim(),
            ToUtc(occurrence.Period.StartTime),
            ToUtc(occurrence.Period.EndTime)
        );
    }

    private static DateTime ToUtc(IDateTime dt) =>
        dt.HasTime ? dt.AsUtc : DateTime.SpecifyKind(dt.Value, DateTimeKind.Utc);
}
