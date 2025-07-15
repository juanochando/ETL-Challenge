namespace EtlChallenge.Contracts.Events.RiskFile;

public record RiskFileParseCompletedEvent
    (Guid CorrelationId, string RiskFileReference, DateTimeOffset? Timestamp = null)
    : BaseEvent(CorrelationId, Timestamp)
{
}
