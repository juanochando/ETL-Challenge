namespace EtlChallenge.Contracts.Events.RiskFile;

public record RiskFileValidationErrorEvent
    (Guid CorrelationId, string RiskFileReference, DateTimeOffset? Timestamp = null)
    : BaseEvent(CorrelationId, Timestamp)
{
}
