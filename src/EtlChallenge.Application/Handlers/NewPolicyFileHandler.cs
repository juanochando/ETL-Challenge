using Microsoft.Extensions.Logging;
using MassTransit;
using EtlChallenge.Contracts.Events.PolicyFile;
using EtlChallenge.StorageService;
using EtlChallenge.Contracts.Events.Policy;
using EtlChallenge.Application.Models;

namespace EtlChallenge.Application.Handlers;

public class NewPolicyFileHandler(IStorageService storageService,
    IPublishEndpoint publishEndpoint,
    ILogger<NewPolicyFileHandler> logger) : IConsumer<PolicyFileAddedEvent>
{
    public async Task Consume(ConsumeContext<PolicyFileAddedEvent> context)
    {
        logger.LogTrace("{CorrelationId} - Processing policy file {PolicyFileReference}", context.Message.CorrelationId, context.Message.PolicyFileReference);

        try
        {
            // Read and parse the policy file one by one
            int count = 0;
            await foreach (var policy in ReadPolicyFileAsync(context.Message.PolicyFileReference))
            {
                count++;
                // Publish each policy as it becomes available
                await publishEndpoint.Publish(new PolicyParsedEvent(
                    context.Message.CorrelationId,
                    context.Message.PolicyFileReference,
                    new Model.Policy
                    {
                        Id = policy.ID,
                        Name = policy.PolicyName,
                        FileStorageReference = context.Message.PolicyFileReference,
                    }));
            }

            logger.LogTrace("Successfully parsed {Count} policies from file {PolicyFileReference}",
                count, context.Message.PolicyFileReference);

            // Publish policy file completed event
            await publishEndpoint.Publish(new PolicyFileParseCompletedEvent(
                context.Message.CorrelationId,
                context.Message.PolicyFileReference));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing policy file {PolicyFileReference}",
                context.Message.PolicyFileReference);

            // Publish policy file parse error event
            await publishEndpoint.Publish(new PolicyFileValidationErrorEvent(
                context.Message.CorrelationId,
                context.Message.PolicyFileReference,
                [ex.Message]));
        }
    }

    private async IAsyncEnumerable<PolicyFileModel> ReadPolicyFileAsync(string fileReference)
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
            if (parts.Length >= 2)
            {
                yield return new PolicyFileModel
                {
                    ID = parts[0].Trim(),
                    PolicyName = parts[1].Trim()
                };
            }
            else
            {
                logger.LogWarning("Invalid line format in policy file: {Line}", line);
                throw new FormatException($"Invalid line format in policy file: {line}");
            }
        }
    }
}
