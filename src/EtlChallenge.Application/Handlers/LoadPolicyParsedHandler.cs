using Microsoft.Extensions.Logging;
using MassTransit;
using EtlChallenge.Contracts.Events.Policy;
using EtlChallenge.ChallengeDB;
using EtlChallenge.Model;

namespace EtlChallenge.Application.Handlers;

public class LoadPolicyParsedHandler(
    ChallengeDBContext dbContext,
    ILogger<LoadPolicyParsedHandler> logger) : IConsumer<PolicyParsedEvent>
{
    public async Task Consume(ConsumeContext<PolicyParsedEvent> context)
    {
        logger.LogTrace("{CorrelationId} - Saving parsed policy item {Id} into staging", context.Message.CorrelationId, context.Message.Policy.Id);

        try
        {
            dbContext.StagedPolicies.Add(new StagedPolicy
            {
                Id = context.Message.Policy.Id,
                Name = context.Message.Policy.Name,
                FileStorageReference = context.Message.PolicyFileReference,
                CorrelationId = context.Message.CorrelationId
            });

            // TODO: Use unit of work and repository
            await dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing policy item {Id}", context.Message.Policy.Id);
        }
    }
}
