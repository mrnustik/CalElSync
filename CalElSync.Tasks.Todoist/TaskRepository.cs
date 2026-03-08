using System.Globalization;
using CalElSync.Core.Tasks;
using CalElSync.Tasks.Todoist.Client;
using CalElSync.Tasks.Todoist.Client.Requests;

namespace CalElSync.Tasks.Todoist;

public class TaskRepository : ITaskRepository
{
    private readonly ITodoistApiClient _todoistApiClient;

    public TaskRepository(ITodoistApiClient todoistApiClient)
    {
        _todoistApiClient = todoistApiClient;
    }

    public async Task<IReadOnlyCollection<TodoTask>> GetTasksAsync(
        string projectId,
        CancellationToken ct
    )
    {
        var tasks = await _todoistApiClient.GetTasksAsync(new TasksFilter(projectId), ct);
        return tasks
            .Where(t => t.Due?.Date != null && t.Due.Date.Contains('T'))
            .Select(t => new TodoTask(
                DateTime.Parse(t.Due!.Date!, null, DateTimeStyles.RoundtripKind).ToUniversalTime(),
                t.Content.Trim()
            ))
            .ToList()
            .AsReadOnly();
    }

    public Task CreateTaskAsync(string projectId, TodoTask task, CancellationToken ct)
    {
        int? durationMinutes = task.Duration.HasValue
            ? (int)task.Duration.Value.TotalMinutes
            : null;
        var request = new CreateTodoistTaskRequest(
            task.Title,
            projectId,
            task.DateTime.ToUniversalTime(),
            durationMinutes,
            durationMinutes.HasValue ? "minute" : null
        );
        return _todoistApiClient.CreateTaskAsync(request, ct);
    }
}
