using CalElSync.Core.Common;
using CalElSync.Core.Events;
using Ical.Net;

namespace CalElSync.Events.iCal;

public class EventsImportService(IFileDownloader fileDownloader) : IEventsImportService
{
    public async Task<IReadOnlyCollection<Event>> GetEventsFromCalendarAsync(
        Uri calendarUrl,
        DateTimeInterval dateTimeInterval,
        CancellationToken ct)
    {
        await using var iCalStream = await fileDownloader.DownloadFileAsync(calendarUrl, ct);
        var calendar = Calendar.Load(iCalStream);
        var occurrences = calendar.GetOccurrences(dateTimeInterval.Start, dateTimeInterval.End);
        return occurrences
            .Select(OccurenceEventsMapper.MapToEvent)
            .ToArray();
    }
}