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

    public async Task<IReadOnlyCollection<TodoTask>> GetTasksAsync(string projectId, CancellationToken ct)
    {
        var tasks = await _todoistApiClient.GetTasksAsync(new TasksFilter(projectId), ct);
        return tasks
            .Where(t => t.Due?.Datetime != null)
            .Select(t => new TodoTask(t.Due!.Datetime!.Value.ToUniversalTime(), t.Content))
            .ToList()
            .AsReadOnly();
    }

    public Task CreateTaskAsync(string projectId, TodoTask task, CancellationToken ct)
    {
        var request = new CreateTodoistTaskRequest(
            task.Title,
            projectId,
            task.DateTime.ToUniversalTime());
        return _todoistApiClient.CreateTaskAsync(request, ct);
    }
}