using MassTransit;
using EtlChallenge.Contracts.Events.Policy;
using EtlChallenge.Contracts.Events.PolicyFile;
using EtlChallenge.Contracts.Events.Risk;
using EtlChallenge.Contracts.Events.RiskFile;

namespace EtlChallenge.Application.Handlers;

// Saga state
public class BatchState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }           // = BatchId
    public HashSet<int> MissingPolicies { get; set; } = new();
    public bool PolicyFileDone { get; set; }
    public bool RiskFileDone { get; set; }
    public string CurrentState { get; set; } = null!;
}

public class BatchSaga : MassTransitStateMachine<BatchState>
{
    public Event<PolicyParsedEvent> PolicyParsed { get; } = null!;
    public Event<RiskParsedEvent> RiskParsed { get; } = null!;
    public Event<PolicyFileParseCompletedEvent> PolicyFileParseCompleted { get; } = null!;
    public Event<RiskFileParseCompletedEvent> RiskFileParseCompleted { get; } = null!;

    State Waiting { get; } = null!;   // until both files done

    public BatchSaga()
    {
        InstanceState(x => x.CurrentState);

        Initially(
            When(PolicyParsed)
                .Then(ctx => ctx.Instance.MissingPolicies.Add(ctx.Data.PolicyId))
                .TransitionTo(Waiting),
            When(RiskParsed)
                .ThenAsync(ctx => HandleRisk(ctx))
                .TransitionTo(Waiting)
        );

        During(Waiting,
            When(PolicyParsed).Then(ctx => ctx.Instance.MissingPolicies.Add(ctx.Data.PolicyId)),
            When(RiskParsed).ThenAsync(ctx => HandleRisk(ctx)),
            When(PolicyFileParseCompleted)
                .Then(ctx => ctx.Instance.PolicyFileDone = true)
                .ThenAsync(ctx => TryFinish(ctx)),
            When(RiskFileParseCompleted)
                .Then(ctx => ctx.Instance.RiskFileDone = true)
                .ThenAsync(ctx => TryFinish(ctx))
        );
    }

    static Task HandleRisk(BehaviorContext<BatchState, RiskParsedEvent> ctx)
    {
        // remove satisfied parent; if parent not yet seen, add to waiting list
        ctx.Instance.MissingPolicies.Remove(ctx.Data.PolicyId);
        if (!ctx.Instance.PolicyFileDone)
            ctx.Instance.MissingPolicies.Add(ctx.Data.PolicyId); // risk before its policy
        return Task.CompletedTask;
    }

    static Task TryFinish(BehaviorContext<BatchState> ctx)
    {
        if (ctx.Instance.PolicyFileDone && ctx.Instance.RiskFileDone)
        {
            if (ctx.Instance.MissingPolicies.Count == 0)
            {
                return ctx.Publish<BatchValidated>(new { ctx.Instance.CorrelationId });
            }
            return ctx.Publish<BatchFailed>(new
            {
                ctx.Instance.CorrelationId,
                Reason = "Orphan risks without parent policies: " +
                         string.Join(',', ctx.Instance.MissingPolicies)
            });
        }
        return Task.CompletedTask;
    }
}
