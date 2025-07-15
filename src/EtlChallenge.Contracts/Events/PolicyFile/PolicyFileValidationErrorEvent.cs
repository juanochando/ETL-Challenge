namespace EtlChallenge.Contracts.Events.PolicyFile;

public record PolicyFileValidationErrorEvent
    (Guid CorrelationId, string PolicyFileReference, string[] Errors, DateTimeOffset? Timestamp = null)
    : BaseEvent(CorrelationId, Timestamp)
{
}
