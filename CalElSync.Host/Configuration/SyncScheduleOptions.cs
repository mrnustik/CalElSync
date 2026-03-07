using System.ComponentModel.DataAnnotations;

namespace CalElSync.Host.Configuration;

public class SyncScheduleOptions
{
    [Required]
    public required string CronExpression { get; init; }
}
