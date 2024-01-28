using CalElSync.Configuration.Table.Extensions;
using CalElSync.Core.Extensions;
using CalElSync.Events.iCal.Extensions;
using CalElSync.Tasks.Todoist.Extensions;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(
        (context, services) =>
        {
            services.AddCalElSync();
            services.AddTodoistTasksIntegration(context.Configuration.GetSection("Todoist"));
            services.AddConfigurationTableStorageIntegration(context.Configuration.GetSection("ConfigurationStorage"));
            services.AddiCalImportIntegration();
        })
    .Build();

host.Run();