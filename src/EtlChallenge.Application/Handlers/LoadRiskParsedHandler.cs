using Microsoft.Extensions.Logging;
using MassTransit;
using EtlChallenge.Contracts.Events.Risk;
using EtlChallenge.ChallengeDB;
using EtlChallenge.Model;

namespace EtlChallenge.Application.Handlers;

public class LoadRiskParsedHandler(
    ChallengeDBContext dbContext,
    ILogger<LoadRiskParsedHandler> logger) : IConsumer<RiskParsedEvent>
{
    public async Task Consume(ConsumeContext<RiskParsedEvent> context)
    {
        logger.LogTrace("{CorrelationId} - Saving parsed risk item {Id} into staging", context.Message.CorrelationId, context.Message.Risk.Id);

        try
        {
            dbContext.StagedRisks.Add(new StagedRisk
            {
                Id = context.Message.Risk.Id,
                Name = context.Message.Risk.Name,
                Peril = context.Message.Risk.Peril,
                PolicyId = context.Message.Risk.PolicyId,
                Street = context.Message.Risk.Street,
                CID = context.Message.Risk.CID,
                Latitude = context.Message.Risk.Latitude,
                Longitude = context.Message.Risk.Longitude,
                FileStorageReference = context.Message.RiskFileReference,
                CorrelationId = context.Message.CorrelationId
            });

            // TODO: Use unit of work and repository
            await dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing risk item {Id}", context.Message.Risk.Id);
        }
    }
}
