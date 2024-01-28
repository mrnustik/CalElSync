﻿namespace CalElSync.Core.Configuration;

public interface ICalendarMappingProvider
{
    Task<IReadOnlyCollection<CalendarProjectMapping>> GetCalendarMappingsAsync(CancellationToken ct);
}