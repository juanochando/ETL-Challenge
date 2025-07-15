using EtlChallenge.Model;

namespace EtlChallenge.Contracts.Events.Risk;

public record RiskParsedEvent
    (Guid CorrelationId, string RiskFileReference, Model.Risk Risk, DateTimeOffset? Timestamp = null)
    : BaseEvent(CorrelationId, Timestamp)
{
}
