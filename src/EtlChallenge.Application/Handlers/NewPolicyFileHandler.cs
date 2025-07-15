using Microsoft.Extensions.Logging;
using MassTransit;
using EtlChallenge.Contracts.Events.PolicyFile;

namespace EtlChallenge.Application.Handlers;

public class NewPolicyFileHandler(ILogger<NewPolicyFileHandler> logger) : IConsumer<PolicyFileAddedEvent>
{
    public Task Consume(ConsumeContext<PolicyFileAddedEvent> context)
    {
        logger.LogTrace("{CorrelationId} - Processing policy file {PolicyFileReference}", context.Message.CorrelationId, context.Message.PolicyFileReference);

        return Task.CompletedTask;
    }
}
