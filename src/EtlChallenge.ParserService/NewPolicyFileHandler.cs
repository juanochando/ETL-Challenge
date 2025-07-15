using MassTransit;
using EtlChallenge.Contracts.Events.PolicyFile;
using EtlChallenge.StorageService;
using EtlChallenge.Contracts.Events.Policy;

namespace EtlChallenge.ParserService;

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
                // Process each policy as it becomes available
                await publishEndpoint.Publish(new PolicyParsedEvent(
                    context.Message.CorrelationId,
                    new Model.Policy
                    {
                        Id = policy.ID,
                        Name = policy.PolicyName,
                    }));
            }

            logger.LogTrace("Successfully parsed {Count} policies from file {PolicyFileReference}",
                count, context.Message.PolicyFileReference);

            // Here you would typically do something with the parsed policies
            // such as saving to a database, publishing events, etc.
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing policy file {PolicyFileReference}",
                context.Message.PolicyFileReference);
        }
    }

    private async IAsyncEnumerable<PolicyFileModel> ReadPolicyFileAsync(string fileReference)
    {
        bool isFirstLine = true;

        var fileContents = storageService.GetFileAsync(fileReference);

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
