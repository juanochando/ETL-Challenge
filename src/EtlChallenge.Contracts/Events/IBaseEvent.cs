namespace EtlChallenge.Contracts.Events;

public interface IBaseEvent
{
    Guid CorrelationId { get; set; }
    DateTimeOffset Timestamp { get; set; }
}
