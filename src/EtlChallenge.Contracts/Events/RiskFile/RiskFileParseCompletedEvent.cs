namespace EtlChallenge.Contracts.Events.RiskFile;

public record RiskFileParseCompletedEvent
    (string RiskFileReference, Guid CorrelationId, DateTimeOffset? Timestamp = null)
    : BaseEvent(CorrelationId, Timestamp)
{
}
