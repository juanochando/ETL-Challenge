namespace EtlChallenge.Contracts.Events.RiskFile;

public record RiskFileLoadedEvent
    (string RiskFileReference, Guid CorrelationId, DateTimeOffset? Timestamp = null)
    : BaseEvent(CorrelationId, Timestamp)
{
}
