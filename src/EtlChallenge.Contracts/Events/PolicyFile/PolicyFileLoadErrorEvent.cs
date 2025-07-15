namespace EtlChallenge.Contracts.Events.PolicyFile;

public record PolicyFileLoadErrorEvent
    (string PolicyFileReference, Guid CorrelationId, DateTimeOffset? Timestamp = null)
    : BaseEvent(CorrelationId, Timestamp)
{
}
