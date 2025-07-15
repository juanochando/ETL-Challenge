using Microsoft.Extensions.Logging;
using MassTransit;
using EtlChallenge.Contracts.Events.RiskFile;
using EtlChallenge.StorageService;
using EtlChallenge.Contracts.Events.Risk;
using EtlChallenge.Model;
using EtlChallenge.Application.Models;

namespace EtlChallenge.Application.Handlers;

public class NewRiskFileHandler(IStorageService storageService,
    IPublishEndpoint publishEndpoint,
    ILogger<NewRiskFileHandler> logger) : IConsumer<RiskFileAddedEvent>
{
    public async Task Consume(ConsumeContext<RiskFileAddedEvent> context)
    {
        logger.LogTrace("{CorrelationId} - Processing Risk file {RiskFileReference}", context.Message.CorrelationId, context.Message.RiskFileReference);

        try
        {
            // Read and parse the Risk file one by one
            int count = 0;
            await foreach (var Risk in ReadRiskFileAsync(context.Message.RiskFileReference))
            {
                count++;
                // Process each Risk as it becomes available
                await publishEndpoint.Publish(new RiskParsedEvent(
                    context.Message.CorrelationId,
                    new Risk
                    {
                        Id = Risk.ID,
                        Name = Risk.RiskName,
                        Peril = Risk.Peril,
                        PolicyId = Risk.PolicyID,
                        Street = Risk.Street,
                        CID = Risk.CID,
                        Latitude = Risk.Latitude,
                        Longitude = Risk.Longitude
                    }));
            }

            logger.LogTrace("Successfully parsed {Count} risks from file {RiskFileReference}",
                count, context.Message.RiskFileReference);

            // Here you would typically do something with the parsed policies
            // such as saving to a database, publishing events, etc.
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing Risk file {RiskFileReference}",
                context.Message.RiskFileReference);
        }
    }

    private async IAsyncEnumerable<RiskFileModel> ReadRiskFileAsync(string fileReference)
    {
        bool isFirstLine = true;

        var fileContents = await storageService.GetFileAsync(fileReference);

        using var fileStream = new MemoryStream(fileContents);
        using var reader = new StreamReader(fileStream);

        string? line;
        while ((line = await reader.ReadLineAsync()) != null)
        {
            // Skip header
            if (isFirstLine)
            {
                isFirstLine = false;
                continue;
            }

            // Skip empty lines
            if (string.IsNullOrWhiteSpace(line))
                continue;

            // Split by tab character
            var parts = line.Split('\t');
            if (parts.Length >= 8) // Expecting 8 columns based on the file format
            {
                yield return new RiskFileModel
                {
                    ID = parts[0],
                    RiskName = parts[1],
                    Peril = Enum.TryParse<Perils>(parts[2], true, out var peril) ? peril : default,
                    PolicyID = parts[3],
                    Street = parts[4],
                    CID = parts[5],
                    Latitude = double.TryParse(parts[6], out var lat) ? lat : 0,
                    Longitude = double.TryParse(parts[7], out var lng) ? lng : 0
                };
            }
            else
            {
                logger.LogWarning("Invalid line format in Risk file: {Line}", line);
                throw new FormatException($"Invalid line format in Risk file: {line}");
            }
        }
    }
}
