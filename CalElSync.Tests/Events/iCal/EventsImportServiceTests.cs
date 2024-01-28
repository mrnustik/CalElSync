using CalElSync.Core.Common;
using CalElSync.Core.Events;
using CalElSync.Events.iCal;
using CalElSync.Tests.Mocks.Events;

namespace CalElSync.Tests.Events.iCal;

public class EventsImportServiceTests
{
    private const string SampleCalendarUrl = "https://outlook.live.com/calendar.ics";
    private const int DayHoursInterval = 24;

    private readonly DateTimeInterval _testingInterval = new DateTimeInterval(
        DateTime.Parse("2024-01-29").ToUniversalTime(),
        DateTime.Parse("2024-02-5").ToUniversalTime());


    [Fact]
    public async Task GetEventsFromCalendarAsync_WithExistingCalendar_ReturnsItsRecurringDailyEvents()
    {
        // Arrange
        var uri = new Uri(SampleCalendarUrl);
        var sut = CreateSut();

        // Act
        var events = await sut.GetEventsFromCalendarAsync(uri, _testingInterval, CancellationToken.None);

        // Assert
        events.Should()
            .ContainEquivalentOf(
                new Event(
                    "Monday Weekly Recurring Time Event",
                    _testingInterval.Start.AddHours(22),
                    _testingInterval.Start.AddHours(22.5)));
    }

    [Fact]
    public async Task GetEventsFromCalendarAsync_WithExistingCalendar_ReturnsItsSingularEvent()
    {
        // Arrange
        var uri = new Uri(SampleCalendarUrl);
        var sut = CreateSut();

        // Act
        var events = await sut.GetEventsFromCalendarAsync(uri, _testingInterval, CancellationToken.None);

        // Assert
        events.Should()
            .ContainEquivalentOf(
                new Event(
                    "Tuesday Morning Event",
                    _testingInterval.Start.AddHours(DayHoursInterval + 8),
                    _testingInterval.Start.AddHours(DayHoursInterval + 10)));
    }

    [Fact]
    public async Task GetEventsFromCalendarAsync_WithExistingCalendar_ReturnsItsAllDayEvent()
    {
        // Arrange
        var uri = new Uri(SampleCalendarUrl);
        var sut = CreateSut();

        // Act
        var events = await sut.GetEventsFromCalendarAsync(uri, _testingInterval, CancellationToken.None);

        // Assert
        events.Should()
            .ContainEquivalentOf(
                new Event(
                    "Wednesday All Day Event",
                    _testingInterval.Start.AddHours(2 * DayHoursInterval),
                    _testingInterval.Start.AddHours(3 * DayHoursInterval)));
    }

    [Fact]
    public async Task GetEventsFromCalendarAsync_WithExistingCalendar_ReturnsItsEveryDayEvent()
    {
        // Arrange
        var uri = new Uri(SampleCalendarUrl);
        var sut = CreateSut();

        // Act
        var events = await sut.GetEventsFromCalendarAsync(uri, _testingInterval, CancellationToken.None);

        // Assert
        for (int day = 0; day < 7; day++)
        {
            events.Should()
                .ContainEquivalentOf(
                    new Event(
                        "Every Day Morning Event",
                        _testingInterval.Start.AddHours(day * DayHoursInterval + 7.5),
                        _testingInterval.Start.AddHours(day * DayHoursInterval + 8)));
        }
    }

    private IEventsImportService CreateSut()
    {
        return new EventsImportService(
            new InMemoryFileDownloader());
    }
}