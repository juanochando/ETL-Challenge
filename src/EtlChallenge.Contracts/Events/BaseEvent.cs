namespace EtlChallenge.Contracts.Events;

public record BaseEvent(Guid CorrelationId, DateTimeOffset? Timestamp = null)
{
}
