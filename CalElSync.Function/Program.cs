using CalElSync.Core.Extensions;
using CalElSync.Events.iCal.Extensions;
using CalElSync.Tasks.Todoist.Client;
using CalElSync.Tasks.Todoist.Extensions;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(services =>
    {
        services.AddCalElSync();
        // TODO Replace by configuration read
        services.AddTodoistTasksIntegration(new TodoistApiOptions());
        services.AddiCalImportIntegration();
    })
    .Build();

host.Run();