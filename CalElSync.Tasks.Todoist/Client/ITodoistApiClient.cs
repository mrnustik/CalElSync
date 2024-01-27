using CalElSync.Tasks.Todoist.Client.Requests;
using CalElSync.Tasks.Todoist.Client.Responses;

namespace CalElSync.Tasks.Todoist.Client;

public interface ITodoistApiClient
{
    Task<IReadOnlyCollection<TodoistTaskResponse>> GetTasksAsync(TasksFilter filter, CancellationToken ct);
    Task CreateTaskAsync(CreateTodoistTaskRequest creationRequest, CancellationToken ct);
}