using CalElSync.Host.Configuration;
using Microsoft.Extensions.Options;

namespace CalElSync.Tests.Configuration;

public class JsonCalendarProjectMappingProviderTests
{
    private static JsonCalendarProjectMappingProvider CreateProvider(string filePath)
    {
        var options = Options.Create(new JsonCalendarMappingOptions { FilePath = filePath });
        return new JsonCalendarProjectMappingProvider(options);
    }

    [Fact]
    public async Task GetCalendarMappingsAsync_WithValidJson_ReturnsCorrectMappings()
    {
        var filePath = Path.GetTempFileName();
        try
        {
            await File.WriteAllTextAsync(filePath, """
                [
                  { "calendarUrl": "https://example.com/calendar.ics", "projectId": "12345" },
                  { "calendarUrl": "https://other.com/events.ics", "projectId": "67890" }
                ]
                """);

            var provider = CreateProvider(filePath);
            var mappings = await provider.GetCalendarMappingsAsync(CancellationToken.None);

            mappings.Should().HaveCount(2);
            mappings.Should().ContainSingle(m =>
                m.CalendarUrl == new Uri("https://example.com/calendar.ics") &&
                m.ProjectId == "12345");
            mappings.Should().ContainSingle(m =>
                m.CalendarUrl == new Uri("https://other.com/events.ics") &&
                m.ProjectId == "67890");
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    [Fact]
    public async Task GetCalendarMappingsAsync_WithEmptyArray_ReturnsEmptyCollection()
    {
        var filePath = Path.GetTempFileName();
        try
        {
            await File.WriteAllTextAsync(filePath, "[]");

            var provider = CreateProvider(filePath);
            var mappings = await provider.GetCalendarMappingsAsync(CancellationToken.None);

            mappings.Should().BeEmpty();
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    [Fact]
    public async Task GetCalendarMappingsAsync_WithMissingFile_ThrowsFileNotFoundException()
    {
        var missingFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".json");
        var provider = CreateProvider(missingFile);

        await provider
            .Invoking(p => p.GetCalendarMappingsAsync(CancellationToken.None))
            .Should().ThrowAsync<FileNotFoundException>();
    }
}
