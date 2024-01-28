using Azure.Data.Tables;
using CalElSync.Core.Configuration;
using Microsoft.Extensions.Options;

namespace CalElSync.Configuration.Table;

public class TableStorageCalendarProjectMappingProvider : ICalendarProjectMappingProvider
{
    private readonly IOptions<ConfigurationTableStorageOptions> _options;

    public TableStorageCalendarProjectMappingProvider(IOptions<ConfigurationTableStorageOptions> options)
    {
        _options = options;
    }

    public async Task<IReadOnlyCollection<CalendarProjectMapping>> GetCalendarMappingsAsync(CancellationToken ct)
    {
        var tableClient = await CreateTableClientAsync(ct);
        var result = new List<CalendarProjectMapping>();
        await foreach (var entity in tableClient.QueryAsync<CalendarProjectMappingEntity>(cancellationToken: ct))
        {
            result.Add(new CalendarProjectMapping(new Uri(entity.CalendarUrl), entity.ProjectId));
        }

        return result;
    }

    private async Task<TableClient> CreateTableClientAsync(CancellationToken ct)
    {
        var tableClient = new TableClient(
            _options.Value.ConnectionString,
            _options.Value.TableName);
        await tableClient.CreateIfNotExistsAsync(ct);
        return tableClient;
    }
}