using System.ComponentModel.DataAnnotations;

namespace CalElSync.Configuration.Table;

public class ConfigurationTableStorageOptions
{
    [Required]
    public required string ConnectionString { get; init; }

    [Required]
    public required string TableName { get; init; }
}