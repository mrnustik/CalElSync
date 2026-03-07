using System.Text.Json;
using CalElSync.Core.Configuration;
using Microsoft.Extensions.Options;

namespace CalElSync.Host.Configuration;

public class JsonCalendarProjectMappingProvider : ICalendarProjectMappingProvider
{
    private readonly IOptions<JsonCalendarMappingOptions> _options;

    public JsonCalendarProjectMappingProvider(IOptions<JsonCalendarMappingOptions> options) =>
        _options = options;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    public async Task<IReadOnlyCollection<CalendarProjectMapping>> GetCalendarMappingsAsync(
        CancellationToken ct
    )
    {
        await using var stream = File.OpenRead(_options.Value.FilePath);
        var entries =
            await JsonSerializer.DeserializeAsync<List<CalendarMappingEntry>>(
                stream,
                JsonOptions,
                ct
            ) ?? [];
        return entries
            .Select(e => new CalendarProjectMapping(new Uri(e.CalendarUrl), e.ProjectId))
            .ToList();
    }

    private record CalendarMappingEntry(string CalendarUrl, string ProjectId);
}
