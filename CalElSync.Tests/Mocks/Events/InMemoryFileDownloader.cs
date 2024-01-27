using System.Reflection;
using CalElSync.Events.iCal;
using Microsoft.Extensions.FileProviders;

namespace CalElSync.Tests.Mocks.Events;

public class InMemoryFileDownloader : IFileDownloader
{
    public Task<Stream> DownloadFileAsync(Uri fileUrl, CancellationToken ct)
    {
        var fileProvider = new EmbeddedFileProvider(Assembly.GetExecutingAssembly());
        var fileInfo = fileProvider.GetFileInfo("Mocks/Events/TestingCalendar.ics");
        var stream = fileInfo.CreateReadStream();
        return Task.FromResult(stream);
    }
}