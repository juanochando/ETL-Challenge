namespace EtlChallenge.Contracts.Events.PolicyFile;

public record PolicyFileValidationErrorEvent
    (string PolicyFileReference, Guid CorrelationId, DateTimeOffset? Timestamp = null)
    : BaseEvent(CorrelationId, Timestamp)
{
}
