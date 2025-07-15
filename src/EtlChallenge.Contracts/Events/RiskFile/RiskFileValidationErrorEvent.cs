namespace EtlChallenge.Contracts.Events.RiskFile;

public record RiskFileValidationErrorEvent
    (string RiskFileReference, Guid CorrelationId, DateTimeOffset? Timestamp = null)
    : BaseEvent(CorrelationId, Timestamp)
{
}
