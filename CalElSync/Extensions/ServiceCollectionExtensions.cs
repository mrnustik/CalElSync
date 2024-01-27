using Microsoft.Extensions.DependencyInjection;

namespace CalElSync.Core.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddCalElSync(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<ISynchronizationHandler, SynchronizationHandler>();
    }
}