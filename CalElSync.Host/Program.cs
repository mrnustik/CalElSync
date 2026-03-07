using CalElSync.Core.Common;
using CalElSync.Core.Configuration;
using CalElSync.Core.Extensions;
using CalElSync.Core.UseCases;
using CalElSync.Events.iCal.Extensions;
using CalElSync.Tasks.Todoist.Extensions;
using CalElSync.Host;
using CalElSync.Host.Configuration;
using Hangfire;
using Hangfire.InMemory;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCalElSync();
builder.Services.AddTodoistTasksIntegration(builder.Configuration.GetSection("Todoist"));
builder.Services.AddTransient<ICalendarProjectMappingProvider, JsonCalendarProjectMappingProvider>();
builder.Services.AddOptions<JsonCalendarMappingOptions>()
    .Bind(builder.Configuration.GetSection("CalendarsFile"))
    .ValidateDataAnnotations()
    .ValidateOnStart();
builder.Services.AddiCalImportIntegration();

builder.Services.AddOptions<SyncScheduleOptions>()
    .Bind(builder.Configuration.GetSection("Sync"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddTransient<SyncJobService>();

builder.Services.AddHangfire(config => config
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseInMemoryStorage());
builder.Services.AddHangfireServer();

var app = builder.Build();

var cron = app.Configuration.GetRequiredSection("Sync")["CronExpression"]!;
app.Services.GetRequiredService<IRecurringJobManager>()
    .AddOrUpdate<SyncJobService>("weekly-sync", job => job.RunAsync(), cron);

app.MapPost("/sync", async (ISynchronizeCalendarEventsToTasks sync, CancellationToken ct) =>
{
    await sync.RunSynchronizationAsync(
        new DateTimeInterval(DateTime.Today, DateTime.Today.AddDays(7)),
        ct);
    return Results.Ok("OK!");
});

app.MapHangfireDashboard("/hangfire");

app.Run();
