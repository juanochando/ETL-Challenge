namespace EltChallenge.UI.Services;

public class FileUploadServiceClient(HttpClient client)
{
    public Task UploadFileAsync(FileStream stream, string fileName)
    {
        string? result = null;

        client.PostAsync($"api/v1/fileUpload", new StreamContent(stream))
            .ContinueWith(response =>
            {
                if (!response.Result.IsSuccessStatusCode)
                {
                    result = $"File upload failed: {response.Result.ReasonPhrase}";
                }
            });

        if (result != null)
        {
            return Task.FromException(new InvalidOperationException(result));
        }
        return Task.CompletedTask;
    }
}
