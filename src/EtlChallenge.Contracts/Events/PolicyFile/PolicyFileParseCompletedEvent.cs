namespace EtlChallenge.Contracts.Events.PolicyFile;

public record PolicyFileParseCompletedEvent
    (Guid CorrelationId, string PolicyFileReference, DateTimeOffset? Timestamp = null)
    : BaseEvent(CorrelationId, Timestamp)
{
}
