using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using EtlChallenge.Application.Commands;

namespace EltChallenge.UI.Pages;

public class IndexModel(
    IMediator mediator,
    ILogger<IndexModel> logger) : PageModel
{
    [BindProperty]
    public IFormFile? UploadedFile { get; set; }

    internal string UploadResult { get; set; } = string.Empty;
    internal string ErrorMessage { get; set; } = string.Empty;

    public async Task OnPostAsync()
    {
        if (UploadedFile == null || UploadedFile.Length == 0)
        {
            ErrorMessage = "Please select a file to upload.";
            logger.LogError("Posted an empty file.");
            return;
        }

        logger.LogTrace("Received file {FileName}.", UploadedFile.FileName);

        try
        {
            using var memoryStream = new MemoryStream();
            await UploadedFile.CopyToAsync(memoryStream);

            // Reset the position to the beginning of the stream for reading
            memoryStream.Position = 0;

            // At this point, we can process the file content in memoryStream
            var commandResponse = await mediator.Send(new NewRawPolicyFile(Guid.NewGuid(), memoryStream));

            if (commandResponse is null)
            {
                logger.LogError("Received empty file process response");
                UploadResult = "Unknown file process error.";
            }

            logger.LogTrace("Received file processed response References {commandResponse.policyFileReference} - {commandResponse.riskFileReference}",
                commandResponse?.PolicyFileReference,
                commandResponse?.RiskFileReference);

            UploadResult = $"File uploaded successfully. References {commandResponse?.PolicyFileReference} - {commandResponse?.RiskFileReference}";
            logger.LogTrace("Result {Result}", UploadResult);
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            logger.LogError(ex, "Error during file upload");
        }
    }
}
