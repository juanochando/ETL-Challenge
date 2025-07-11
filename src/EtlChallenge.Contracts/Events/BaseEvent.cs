namespace EtlChallenge.Contracts.Events;

public class BaseEvent : IBaseEvent
{
    public required Guid CorrelationId { get; set; }
    public required DateTimeOffset Timestamp { get; set; }
}
