namespace CalElSync.Events.iCal;

public interface IFileDownloader
{
    Task<Stream> DownloadFileAsync(Uri fileUrl, CancellationToken ct);
}