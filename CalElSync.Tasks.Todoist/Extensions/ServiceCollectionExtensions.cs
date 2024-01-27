using CalElSync.Core.Tasks;
using CalElSync.Tasks.Todoist.Client;
using Microsoft.Extensions.DependencyInjection;

namespace CalElSync.Tasks.Todoist.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTodoistTasksIntegration(
        this IServiceCollection serviceCollection,
        TodoistApiOptions options)
    {
        serviceCollection.AddTransient<ITaskRepository, TaskRepository>();
        serviceCollection.AddTransient<ITodoistApiClient, TodoistApiClient>();
        serviceCollection.AddHttpClient<ITodoistApiClient, TodoistApiClient>(
                client =>
                    client.BaseAddress = new Uri("https://api.todoist.com/"))
            .AddHttpMessageHandler<TodoistApiAuthorizationHandler>();
        serviceCollection.AddTransient<TodoistApiAuthorizationHandler>();
        serviceCollection.AddOptions<TodoistApiOptions>()
            .Configure(apiOptions => apiOptions.ApiKey = options.ApiKey);
        return serviceCollection;
    }
}