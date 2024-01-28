using CalElSync.Core;
using CalElSync.Core.Common;
using CalElSync.Core.Configuration;
using CalElSync.Core.Tasks;
using CalElSync.Events.iCal;
using CalElSync.Tests.Mocks.Configuration;
using CalElSync.Tests.Mocks.Events;
using CalElSync.Tests.Mocks.Tasks;
using Microsoft.Extensions.Logging.Abstractions;

namespace CalElSync.Tests;

public class SynchronizationHandlerTests
{
    private const int DayHoursInterval = 24;
    private const string SampleCalendarUrl = "https://outlook.live.com/calendar.ics";

    private readonly InMemoryCalendarMappingProvider _inMemoryCalendarMappingProvider = new();
    private readonly InMemoryTaskRepository _inMemoryTaskRepository = new();

    private readonly DateTimeInterval _testingInterval = new DateTimeInterval(
        DateTime.Parse("2024-01-29").ToUniversalTime(),
        DateTime.Parse("2024-02-5").ToUniversalTime());

    private IReadOnlyCollection<TodoTask> ExpectedTasks => new[]
    {
        new TodoTask(_testingInterval.Start.AddHours(0 * DayHoursInterval + 7.5), "Every Day Morning Event"),
        new TodoTask(_testingInterval.Start.AddHours(0 * DayHoursInterval + 22), "Monday Weekly Recurring Time Event"),
        new TodoTask(_testingInterval.Start.AddHours(1 * DayHoursInterval + 7.5), "Every Day Morning Event"),
        new TodoTask(_testingInterval.Start.AddHours(1 * DayHoursInterval + 8), "Tuesday Morning Event"),
        new TodoTask(_testingInterval.Start.AddHours(2 * DayHoursInterval + 7.5), "Every Day Morning Event"),
        new TodoTask(_testingInterval.Start.AddHours(2 * DayHoursInterval + 0), "Wednesday All Day Event"),
        new TodoTask(_testingInterval.Start.AddHours(3 * DayHoursInterval + 7.5), "Every Day Morning Event"),
        new TodoTask(_testingInterval.Start.AddHours(4 * DayHoursInterval + 7.5), "Every Day Morning Event"),
        new TodoTask(_testingInterval.Start.AddHours(5 * DayHoursInterval + 7.5), "Every Day Morning Event"),
        new TodoTask(_testingInterval.Start.AddHours(6 * DayHoursInterval + 7.5), "Every Day Morning Event"),
    };

    [Fact]
    public async Task RunSynchronization_WithEmptyTaskProject_CreatesThemAll()
    {
        // Arrange
        var projectId = "test-project-id";
        var calendarUrl = new Uri(SampleCalendarUrl);
        _inMemoryCalendarMappingProvider.AddMapping(
            new CalendarProjectMapping(calendarUrl, projectId));
        var synchronizationHandler = CreateSut();

        // Act
        await synchronizationHandler.RunSynchronizationAsync(_testingInterval, CancellationToken.None);

        // Assert
        var tasks = await _inMemoryTaskRepository.GetTasksAsync(projectId, CancellationToken.None);
        tasks
            .Should()
            .HaveCount(10);

        tasks.Should()
            .BeEquivalentTo(ExpectedTasks);
    }

    [Fact]
    public async Task RunSynchronization_WithMatchingExistingTask_DoesNotRecreateExisting()
    {
        // Arrange
        var projectId = "test-project-id";
        var calendarUrl = new Uri(SampleCalendarUrl);
        _inMemoryCalendarMappingProvider.AddMapping(
            new CalendarProjectMapping(calendarUrl, projectId));
        _inMemoryTaskRepository.AddTask(
            projectId,
            new TodoTask(
                _testingInterval.Start.AddHours(0 * DayHoursInterval + 22),
                "Monday Weekly Recurring Time Event"));
        var synchronizationHandler = CreateSut();

        // Act
        await synchronizationHandler.RunSynchronizationAsync(_testingInterval, CancellationToken.None);

        // Assert
        var tasks = await _inMemoryTaskRepository.GetTasksAsync(projectId, CancellationToken.None);
        tasks
            .Should()
            .HaveCount(10);

        tasks.Should()
            .BeEquivalentTo(ExpectedTasks);
    }

    [Fact]
    public async Task RunSynchronization_WithNotMatchingExistingTask_CreatesAllMissingTasks()
    {
        // Arrange
        var projectId = "test-project-id";
        var calendarUrl = new Uri(SampleCalendarUrl);
        _inMemoryCalendarMappingProvider.AddMapping(
            new CalendarProjectMapping(calendarUrl, projectId));
        var existingTaskWithMatchingTitle = new TodoTask(
            _testingInterval.Start.AddHours(7),
            "Monday Weekly Recurring Time Event");
        var existingTaskWithMatchingDate = new TodoTask(
            _testingInterval.Start.AddHours(0 * DayHoursInterval + 22),
            "Monday Weekly Recurring Time Event With not matching title");
        _inMemoryTaskRepository.AddTask(projectId, existingTaskWithMatchingTitle);
        _inMemoryTaskRepository.AddTask(projectId, existingTaskWithMatchingDate);
        var synchronizationHandler = CreateSut();

        // Act
        await synchronizationHandler.RunSynchronizationAsync(_testingInterval, CancellationToken.None);

        // Assert
        var tasks = await _inMemoryTaskRepository.GetTasksAsync(projectId, CancellationToken.None);
        tasks
            .Should()
            .HaveCount(12);

        tasks.Should()
            .BeEquivalentTo(
                ExpectedTasks
                    .Concat(new[] { existingTaskWithMatchingDate, existingTaskWithMatchingTitle }));
    }

    private ISynchronizationHandler CreateSut()
    {
        return new SynchronizationHandler(
            new EventsImportService(
                new InMemoryFileDownloader()),
            _inMemoryCalendarMappingProvider,
            _inMemoryTaskRepository,
            NullLogger<SynchronizationHandler>.Instance);
    }
}