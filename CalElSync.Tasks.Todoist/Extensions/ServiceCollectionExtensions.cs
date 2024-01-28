using CalElSync.Core.Tasks;
using CalElSync.Tasks.Todoist.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CalElSync.Tasks.Todoist.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTodoistTasksIntegration(
        this IServiceCollection serviceCollection,
        IConfigurationSection options)
    {
        serviceCollection.AddTransient<ITaskRepository, TaskRepository>();
        serviceCollection.AddTransient<ITodoistApiClient, TodoistApiClient>();
        serviceCollection.AddHttpClient<ITodoistApiClient, TodoistApiClient>(
                client =>
                    client.BaseAddress = new Uri("https://api.todoist.com/"))
            .AddHttpMessageHandler<TodoistApiAuthorizationHandler>();
        serviceCollection.AddTransient<TodoistApiAuthorizationHandler>();
        serviceCollection.AddOptions<TodoistApiOptions>()
            .Bind(options)
            .ValidateDataAnnotations()
            .ValidateOnStart();
        return serviceCollection;
    }
}