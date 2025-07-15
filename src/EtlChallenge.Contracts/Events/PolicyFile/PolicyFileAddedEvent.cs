namespace EtlChallenge.Contracts.Events.PolicyFile;

public record PolicyFileAddedEvent
    (string PolicyFileReference, Guid CorrelationId, DateTimeOffset? Timestamp = null)
    : BaseEvent(CorrelationId, Timestamp)
{
}
