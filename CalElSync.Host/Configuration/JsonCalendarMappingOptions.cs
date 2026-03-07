using System.ComponentModel.DataAnnotations;

namespace CalElSync.Host.Configuration;

public class JsonCalendarMappingOptions
{
    [Required]
    public required string FilePath { get; init; }
}
