using CalElSync.Core.Common;
using CalElSync.Core.Configuration;
using CalElSync.Core.Extensions;
using CalElSync.Core.UseCases;
using CalElSync.Events.iCal.Extensions;
using CalElSync.Tasks.Todoist.Extensions;
using CalElSync.Host;
using CalElSync.Host.Configuration;
using Quartz;

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

builder.Services.AddQuartz(q =>
{
    var cron = builder.Configuration.GetRequiredSection("Sync")["CronExpression"]!;
    q.ScheduleJob<SyncJob>(trigger => trigger
        .WithCronSchedule(cron, x => x.InTimeZone(TimeZoneInfo.Utc))
        .WithIdentity("sync-trigger"));
});
builder.Services.AddQuartzHostedService(opt => opt.WaitForJobsToComplete = true);

var app = builder.Build();

app.MapPost("/sync", async (ISynchronizeCalendarEventsToTasks sync, CancellationToken ct) =>
{
    await sync.RunSynchronizationAsync(
        new DateTimeInterval(DateTime.Today, DateTime.Today.AddDays(7)),
        ct);
    return Results.Ok("OK!");
});

app.Run();
