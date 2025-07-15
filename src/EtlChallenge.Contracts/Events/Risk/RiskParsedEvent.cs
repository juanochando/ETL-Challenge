namespace EtlChallenge.Contracts.Events.Risk;

public record RiskParsedEvent
    (Guid CorrelationId, DateTimeOffset? Timestamp = null)
    : BaseEvent(CorrelationId, Timestamp)
{
}
