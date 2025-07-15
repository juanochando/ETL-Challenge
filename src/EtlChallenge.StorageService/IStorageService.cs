namespace EtlChallenge.StorageService;

public interface IStorageService
{
    /// <summary>
    /// Stores a file's contents permanently.
    /// </summary>
    /// <param name="fileName">The name of the file to upload.</param>
    /// <param name="fileContent">The content of the file as a stream.</param>
    /// <returns>A task that represents the asynchronous operation, containing a string holding the file reference.</returns>
    Task<string> UploadFileAsync(string fileName, Stream fileContent);

    /// <summary>
    /// Retrieves a file's contents from the permanent storage.
    /// </summary>
    /// <param name="fileReference">The reference of the file to retrieve.</param>
    /// <returns>A task that represents the asynchronous operation, containing the file content as a stream.</returns>
    byte[] GetFileAsync(string fileReference);
}
