using CalElSync.Configuration.Table.Extensions;
using CalElSync.Core.Common;
using CalElSync.Core.Extensions;
using CalElSync.Core.UseCases;
using CalElSync.Events.iCal.Extensions;
using CalElSync.Tasks.Todoist.Extensions;
using CalElSync.Host;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCalElSync();
builder.Services.AddTodoistTasksIntegration(builder.Configuration.GetSection("Todoist"));
builder.Services.AddConfigurationTableStorageIntegration(builder.Configuration.GetSection("ConfigurationStorage"));
builder.Services.AddiCalImportIntegration();
builder.Services.AddHostedService<WeeklySyncService>();

var app = builder.Build();

app.MapPost("/sync", async (ISynchronizeCalendarEventsToTasks sync, CancellationToken ct) =>
{
    await sync.RunSynchronizationAsync(
        new DateTimeInterval(DateTime.Today, DateTime.Today.AddDays(7)),
        ct);
    return Results.Ok("OK!");
});

app.Run();
