using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace EtlChallenge.StorageService;

// TODO: Move to the DB or Azure Blob sotrage
public class StorageService(ILogger<StorageService> logger) : IStorageService
{
    private readonly ConcurrentDictionary<string, byte[]> _fileStorage = new();

    public async Task<string> UploadFileAsync(string fileName, Stream fileContent)
    {
        logger.LogTrace("Uploading file {FileName}", fileName);

        // Generate a unique reference for the file
        string fileReference = $"{Guid.NewGuid()}_{fileName}";

        // Copy the file content to a memory stream
        using var memoryStream = new MemoryStream();
        await fileContent.CopyToAsync(memoryStream);
        byte[] fileBytes = memoryStream.ToArray();

        // Store the file in the dictionary
        _fileStorage[fileReference] = fileBytes;

        return fileReference;
    }

    public byte[] GetFileAsync(string fileReference)
    {
        if (_fileStorage.TryGetValue(fileReference, out var fileContent))
        {
            return fileContent;
        }

        throw new FileNotFoundException($"File with reference {fileReference} not found.");
    }
}
