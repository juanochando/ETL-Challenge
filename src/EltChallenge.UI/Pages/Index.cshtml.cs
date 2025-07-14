using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using EltChallenge.UI.Services;

namespace EltChallenge.UI.Pages;

public class IndexModel(
    ILogger<IndexModel> _logger,
    IHostEnvironment _environment,
    FileUploadServiceClient fileUploadServiceClient) : PageModel
{
    [BindProperty]
    public IFormFile? UploadedFile { get; set; }

    internal string UploadResult { get; set; } = string.Empty;
    internal string ErrorMessage { get; set; } = string.Empty;

    public async void OnPostAsync()
    {
        if (UploadedFile == null || UploadedFile.Length == 0)
        {
            _logger.LogError("Posted an empty file.");
            return;
        }

        _logger.LogInformation("Uploading {FileName}.", UploadedFile.FileName);

        string targetFileName = $"{_environment.ContentRootPath}/{UploadedFile.FileName}";

        using var stream = new FileStream(targetFileName, FileMode.Create);
        await UploadedFile.CopyToAsync(stream);

        await fileUploadServiceClient.UploadFileAsync(stream, UploadedFile.FileName)
            .ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    ErrorMessage = task.Exception?.Message ?? "An error occurred during file upload.";
                    _logger.LogError("Error {Error}", ErrorMessage);
                }
                else
                {
                    UploadResult = "File uploaded successfully.";
                    _logger.LogInformation("Result {Result}", UploadResult);
                }
            });
    }
}
