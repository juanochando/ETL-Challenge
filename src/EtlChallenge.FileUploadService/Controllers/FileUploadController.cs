using Microsoft.AspNetCore.Mvc;

namespace EtlChallenge.FileUploadService.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class FileUploadController(ILogger<FileUploadController> logger) : ControllerBase
{
    [HttpPost()]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UploadFile()
    {
        try
        {
            using var memoryStream = new MemoryStream();

            // Copy the request body to a memory stream
            await Request.Body.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            // At this point, we can process the file content in memoryStream

            // Log file information
            var fileSize = memoryStream.Length;
            logger.LogTrace("Received file of size: {FileSize} bytes", fileSize);

            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }
}
