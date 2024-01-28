using CalElSync.Core.Tasks;
using CalElSync.Tasks.Todoist.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CalElSync.Tests.Tasks;

public class TaskRepositoryTests
{
    public const string ApiKey = "REPLACE_WITH_REAL_KEY";
    public const string ProjectId = "123456789";

    [Fact(Skip = "These tests rely on the Todoist API")]
    public async Task GetTasksForProject_WithExistingProject_ReturnsThem()
    {
        // Arrange
        var taskRepository = CreateSut();

        // Act
        var tasks = await taskRepository.GetTasksAsync(ProjectId, CancellationToken.None);

        // Assert
        tasks.Should().HaveCount(8);
    }

    [Fact(Skip = "These tests rely on the Todoist API")]
    public async Task CreateTask_WithExistingProject_CreatesIt()
    {
        // Arrange
        var taskRepository = CreateSut();

        // Act
        await taskRepository.CreateTaskAsync(
            ProjectId,
            new TodoTask(DateTime.Now, "Test"),
            CancellationToken.None);
    }

    private ITaskRepository CreateSut()
    {
        var serviceCollection = new ServiceCollection();
        var configurationBuilder = new ConfigurationBuilder();
        var configurationRoot = configurationBuilder.AddInMemoryCollection(
                new[]
                {
                    new KeyValuePair<string, string?>("Todoist:ApiKey", ApiKey)
                })
            .Build();
        serviceCollection.AddTodoistTasksIntegration(configurationRoot.GetRequiredSection("Todoist"));
        var serviceProvider = serviceCollection.BuildServiceProvider();
        return serviceProvider.GetRequiredService<ITaskRepository>();
    }
}