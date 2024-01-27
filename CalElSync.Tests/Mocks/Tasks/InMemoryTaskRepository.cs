using CalElSync.Core.Tasks;

namespace CalElSync.Tests.Mocks.Tasks;

public class InMemoryTaskRepository : ITaskRepository
{
    private readonly IDictionary<string, ICollection<TodoTask>> _tasks = new Dictionary<string, ICollection<TodoTask>>();
    
    public Task<IReadOnlyCollection<TodoTask>> GetTasksAsync(string projectId, CancellationToken ct)
    {
        if (!_tasks.TryGetValue(projectId, out var projectTasks))
        {
            return Task.FromResult<IReadOnlyCollection<TodoTask>>(Array.Empty<TodoTask>());
        }
        return Task.FromResult<IReadOnlyCollection<TodoTask>>(projectTasks
            .ToArray()
            .AsReadOnly());
    }

    public Task CreateTaskAsync(string projectId, TodoTask task, CancellationToken ct)
    {
        AddTask(projectId, task);
        return Task.CompletedTask;
    }

    public void AddTask(string projectId, TodoTask task)
    {
        if (_tasks.TryGetValue(projectId, out var existingCollection))
        {
            existingCollection.Add(task);
        }
        else
        {
            var newCollection = new List<TodoTask>
            {
                task
            };
            _tasks.Add(projectId, newCollection);
        }
    }
}