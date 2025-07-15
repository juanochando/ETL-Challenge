using EtlChallenge.Model;

namespace EtlChallenge.Contracts.Events.Policy;

public record PolicyParsedEvent
    (Guid CorrelationId, Model.Policy Policy, DateTimeOffset? Timestamp = null)
    : BaseEvent(CorrelationId, Timestamp)
{
}
