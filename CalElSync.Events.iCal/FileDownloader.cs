using Microsoft.Extensions.Logging;

namespace CalElSync.Events.iCal;

public class FileDownloader(
    ILogger<FileDownloader> logger,
    HttpClient httpClient)
    : IFileDownloader
{
    public async Task<Stream> DownloadFileAsync(Uri fileUrl, CancellationToken ct)
    {
        try
        {
            logger.LogInformation("Downloading file from {FileUrl}", fileUrl);
            var response = await httpClient.GetAsync(fileUrl, ct);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStreamAsync(ct);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "An error occured while downloading file from {FileUrl}", fileUrl);
            throw;
        }
    }
}