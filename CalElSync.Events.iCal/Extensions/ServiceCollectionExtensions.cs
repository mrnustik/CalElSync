using CalElSync.Core.Events;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;

namespace CalElSync.Events.iCal.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddiCalImportIntegration(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<IFileDownloader, FileDownloader>();
        serviceCollection.AddHttpClient<IFileDownloader, FileDownloader>()
            .AddPolicyHandler(HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))));
        serviceCollection.AddTransient<IEventsImportService, EventsImportService>();
    }
}