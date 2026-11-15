using Microsoft.Extensions.Configuration;

namespace Octo.Infrastructure.Storage.Services;

public class LocalStorageProvider : IStorageService
{
    private readonly string _baseStoragePath;

    IConfiguration configuration;

    public LocalStorageProvider(IConfiguration configuration)
    {
        _baseStoragePath = configuration["Storage:LocalPath"] ?? Path.Combine(Directory.GetCurrentDirectory(), "FilesStorage");

        if (!Directory.Exists(_baseStoragePath))
        {
            Directory.CreateDirectory(_baseStoragePath);
        }
    }

    public async Task<string> UploadAsync(string fileName, Stream fileStream, string containerName, CancellationToken cancellationToken = default)
    {
        var containerPath = Path.Combine(_baseStoragePath, containerName);
        if (!Directory.Exists(containerPath))
        {
            Directory.CreateDirectory(containerPath);
        }

        var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
        var fullPath = Path.Combine(containerPath, uniqueFileName);

        await using var targetStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write);
        await fileStream.CopyToAsync(targetStream, cancellationToken);

        // Retorna o caminho relativo ou identificador para armazenamento no banco
        return Path.Combine(containerName, uniqueFileName).Replace("\\", "/");
    }

    public Task<Stream> DownloadAsync(string filePath, CancellationToken cancellationToken = default)
    {
        var fullPath = Path.Combine(_baseStoragePath, filePath);

        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException($"O arquivo {filePath} não foi encontrado no storage local.");
        }

        Stream stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
        return Task.FromResult(stream);
    }

    public Task DeleteAsync(string filePath, CancellationToken cancellationToken = default)
    {
        var fullPath = Path.Combine(_baseStoragePath, filePath);

        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }

        return Task.CompletedTask;
    }

    public Task<string> GetUrlAsync(string filePath, CancellationToken cancellationToken = default)
    {
        // No storage local, retornamos o caminho relativo ou endpoint de stream interno
        return Task.FromResult($"/api/storage/download?path={filePath}");
    }
}