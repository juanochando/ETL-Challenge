namespace EtlChallenge.Contracts.Events.RiskFile;

public record RiskFileLoadErrorEvent
    (string RiskFileReference, Guid CorrelationId, DateTimeOffset? Timestamp = null)
    : BaseEvent(CorrelationId, Timestamp)
{
}
