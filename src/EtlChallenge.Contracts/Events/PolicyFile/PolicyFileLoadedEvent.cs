namespace EtlChallenge.Contracts.Events.PolicyFile;

public record PolicyFileLoadedEvent
    (string PolicyFileReference, Guid CorrelationId, DateTimeOffset? Timestamp = null)
    : BaseEvent(CorrelationId, Timestamp)
{
}
