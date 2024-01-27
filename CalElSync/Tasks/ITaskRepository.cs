namespace CalElSync.Core.Tasks;

public interface ITaskRepository
{
    Task<IReadOnlyCollection<TodoTask>> GetTasksAsync(string projectId, CancellationToken ct);
    Task CreateTaskAsync(string projectId, TodoTask task, CancellationToken ct);
}