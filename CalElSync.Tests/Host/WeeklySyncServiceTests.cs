using CalElSync.Host;
using CalElSync.Host.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Time.Testing;

namespace CalElSync.Tests.Host;

public class WeeklySyncServiceTests
{
    private static readonly IServiceScopeFactory ScopeFactory =
        new ServiceCollection().BuildServiceProvider().GetRequiredService<IServiceScopeFactory>();

    private static WeeklySyncService CreateSut(FakeTimeProvider timeProvider, string cronExpression = "0 2 * * 1") =>
        new(ScopeFactory,
            NullLogger<WeeklySyncService>.Instance,
            Options.Create(new SyncScheduleOptions { CronExpression = cronExpression }),
            timeProvider);

    [Fact]
    public void GetNextOccurrence_WhenMondayBeforeScheduledTime_ReturnsSameDayAtScheduledTime()
    {
        // Monday 2024-01-29 at 01:00 UTC — before the 02:00 schedule
        var fakeTime = new FakeTimeProvider(new DateTimeOffset(2024, 1, 29, 1, 0, 0, TimeSpan.Zero));

        var next = CreateSut(fakeTime).GetNextOccurrence();

        next.Should().Be(new DateTimeOffset(2024, 1, 29, 2, 0, 0, TimeSpan.Zero));
    }

    [Fact]
    public void GetNextOccurrence_WhenMondayAfterScheduledTime_ReturnsNextMonday()
    {
        // Monday 2024-01-29 at 03:00 UTC — past the 02:00 schedule
        var fakeTime = new FakeTimeProvider(new DateTimeOffset(2024, 1, 29, 3, 0, 0, TimeSpan.Zero));

        var next = CreateSut(fakeTime).GetNextOccurrence();

        next.Should().Be(new DateTimeOffset(2024, 2, 5, 2, 0, 0, TimeSpan.Zero));
    }

    [Fact]
    public void GetNextOccurrence_WhenMidWeek_ReturnsNextMonday()
    {
        // Wednesday 2024-01-31 at 12:00 UTC
        var fakeTime = new FakeTimeProvider(new DateTimeOffset(2024, 1, 31, 12, 0, 0, TimeSpan.Zero));

        var next = CreateSut(fakeTime).GetNextOccurrence();

        next.Should().Be(new DateTimeOffset(2024, 2, 5, 2, 0, 0, TimeSpan.Zero));
    }
}
