namespace Octo.Infrastructure.Storage.Services;

public interface IStorageService
{
    Task<string> UploadAsync(string fileName, Stream fileStream, string containerName, CancellationToken cancellationToken = default);
    Task<Stream> DownloadAsync(string filePath, CancellationToken cancellationToken = default);
    Task DeleteAsync(string filePath, CancellationToken cancellationToken = default);
    Task<string> GetUrlAsync(string filePath, CancellationToken cancellationToken = default);
}