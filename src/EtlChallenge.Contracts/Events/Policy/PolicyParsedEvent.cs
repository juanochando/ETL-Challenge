using EtlChallenge.Model;

namespace EtlChallenge.Contracts.Events.Policy;

public record PolicyParsedEvent
    (Guid CorrelationId, string PolicyFileReference, Model.Policy Policy, DateTimeOffset? Timestamp = null)
    : BaseEvent(CorrelationId, Timestamp)
{
}
