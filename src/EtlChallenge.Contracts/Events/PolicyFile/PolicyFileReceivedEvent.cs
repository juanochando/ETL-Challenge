namespace EtlChallenge.Contracts.Events.PolicyFile;

public record PolicyFileReceivedEvent
    (string PolicyFileReference, Guid CorrelationId, DateTimeOffset? Timestamp = null)
    : BaseEvent(CorrelationId, Timestamp)
{
}
