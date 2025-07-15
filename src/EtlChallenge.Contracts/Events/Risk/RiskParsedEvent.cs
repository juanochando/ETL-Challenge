using EtlChallenge.Model;

namespace EtlChallenge.Contracts.Events.Risk;

public record RiskParsedEvent
    (Guid CorrelationId, Model.Risk Risk, DateTimeOffset? Timestamp = null)
    : BaseEvent(CorrelationId, Timestamp)
{
}
