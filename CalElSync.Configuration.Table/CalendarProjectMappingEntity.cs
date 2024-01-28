using Azure;
using Azure.Data.Tables;

namespace CalElSync.Configuration.Table;

public class CalendarProjectMappingEntity : ITableEntity
{
    public required string PartitionKey { get; set; }
    public required string RowKey { get; set; }
    public required string CalendarUrl { get; set; }
    public required string ProjectId { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
}