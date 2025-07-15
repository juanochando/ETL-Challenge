using Azure.Storage.Blobs;
using EtlChallenge.Contracts.Application;
using Microsoft.Extensions.Logging;

namespace EtlChallenge.StorageService;

public class StorageService(
    BlobServiceClient blobServiceClient,
    ILogger<StorageService> logger) : IStorageService
{
    public async Task<string> UploadFileAsync(string fileName, Stream fileContent)
    {
        logger.LogTrace("Storing file {FileName}", fileName);

        // Generate a unique reference for the file
        string fileReference = $"{Guid.NewGuid()}_{fileName}";

        // Get a reference to a container (create if it doesn't exist)
        var containerClient = blobServiceClient.GetBlobContainerClient(Constants.StorageService_FilesContainer);
        await containerClient.CreateIfNotExistsAsync();

        // Get a reference to a blob
        var blobClient = containerClient.GetBlobClient(fileReference);

        // Upload the file to Azure Blob Storage
        await blobClient.UploadAsync(fileContent, true);

        return fileReference;
    }

    public async Task<byte[]> GetFileAsync(string fileReference)
    {
        logger.LogTrace("Retrieving file {FileReference}", fileReference);

        // Get a reference to the container
        var containerClient = blobServiceClient.GetBlobContainerClient(Constants.StorageService_FilesContainer);

        // Get a reference to the blob
        var blobClient = containerClient.GetBlobClient(fileReference);

        // Check if the blob exists
        var exists = await blobClient.ExistsAsync();
        if (!exists.Value)
        {
            throw new FileNotFoundException($"File with reference {fileReference} not found.");
        }

        // Download the blob content
        using var memoryStream = new MemoryStream();
        await blobClient.DownloadToAsync(memoryStream);
        return memoryStream.ToArray();
    }
}
