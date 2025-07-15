namespace EtlChallenge.Contracts.Events.RiskFile;

public record RiskFileAddedEvent
    (string RiskFileReference, Guid CorrelationId, DateTimeOffset? Timestamp = null)
    : BaseEvent(CorrelationId, Timestamp)
{
}
