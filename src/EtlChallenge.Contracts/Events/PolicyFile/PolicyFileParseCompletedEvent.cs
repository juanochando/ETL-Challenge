namespace EtlChallenge.Contracts.Events.PolicyFile;

public record PolicyFileParseCompletedEvent
    (string PolicyFileReference, Guid CorrelationId, DateTimeOffset? Timestamp = null)
    : BaseEvent(CorrelationId, Timestamp)
{
}
