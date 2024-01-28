using CalElSync.Core.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CalElSync.Configuration.Table.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddConfigurationTableStorageIntegration(
        this IServiceCollection serviceCollection,
        IConfigurationSection tableStorageConfiguration)
    {
        serviceCollection.AddTransient<ICalendarProjectMappingProvider, TableStorageCalendarProjectMappingProvider>();
        serviceCollection.AddOptions<ConfigurationTableStorageOptions>()
            .Bind(tableStorageConfiguration)
            .ValidateDataAnnotations()
            .ValidateOnStart();
    }
}