using CalElSync.Core.Common;
using CalElSync.Core.Configuration;
using CalElSync.Core.Events;
using CalElSync.Core.Tasks;
using Microsoft.Extensions.Logging;

namespace CalElSync.Core;

public class SynchronizationHandler : ISynchronizationHandler
{
    private readonly IEventsImportService _eventsImportService;
    private readonly ICalendarMappingProvider _calendarMappingProvider;
    private readonly ITaskRepository _taskRepository;
    private readonly ILogger<SynchronizationHandler> _logger;

    public SynchronizationHandler(
        IEventsImportService eventsImportService,
        ICalendarMappingProvider calendarMappingProvider,
        ITaskRepository taskRepository,
        ILogger<SynchronizationHandler> logger)
    {
        _eventsImportService = eventsImportService;
        _calendarMappingProvider = calendarMappingProvider;
        _logger = logger;
        _taskRepository = taskRepository;
    }

    public async Task RunSynchronizationAsync(DateTimeInterval interval, CancellationToken ct)
    {
        var calendarMappings = await _calendarMappingProvider.GetCalendarMappingsAsync(ct);
        foreach (var calendarProjectMapping in calendarMappings)
        {
            try
            {
                await RunSynchronizationForMappingAsync(calendarProjectMapping, interval, ct);
            }
            catch (Exception exception)
            {
                _logger.LogError(
                    exception,
                    "An unhandled exception occured when trying to map calendar {CalendarUrl} to project {ProjectId}",
                    calendarProjectMapping.CalendarUrl,
                    calendarProjectMapping.ProjectId);
            }
        }
    }

    private async Task RunSynchronizationForMappingAsync(
        CalendarProjectMapping calendarProjectMapping,
        DateTimeInterval interval,
        CancellationToken ct)
    {
        var events = await _eventsImportService.GetEventsFromCalendarAsync(
            calendarProjectMapping.CalendarUrl,
            interval,
            ct);
        var existingTasks = await _taskRepository.GetTasksAsync(calendarProjectMapping.ProjectId, ct);
        foreach (var calendarEvent in events)
        {
            if (!existingTasks.Any(
                    t =>
                        t.Title == calendarEvent.Title &&
                        t.DateTime == calendarEvent.StartTime))
            {
                var taskToCreate = new TodoTask(calendarEvent.StartTime, calendarEvent.Title);
                await _taskRepository.CreateTaskAsync(
                    calendarProjectMapping.ProjectId,
                    taskToCreate,
                    ct);
                _logger.LogInformation(
                    "Created task {TaskTitle} in project {ProjectId}",
                    calendarEvent.Title,
                    calendarProjectMapping.ProjectId);
            }
        }
    }
}