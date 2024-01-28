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
            occurrence.Period.StartTime.Value.ToUniversalTime(),
            occurrence.Period.EndTime.Value.ToUniversalTime());
    }
}