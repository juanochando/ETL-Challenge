namespace EtlChallenge.Contracts.Events.Policy;

public record PolicyParsedEvent
    (Guid CorrelationId, DateTimeOffset? Timestamp = null)
    : BaseEvent(CorrelationId, Timestamp)
{
}
