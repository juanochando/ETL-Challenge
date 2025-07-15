using Microsoft.Extensions.Logging;
using MassTransit;
using EtlChallenge.Contracts.Events.Policy;
using EtlChallenge.Contracts.Events.PolicyFile;
using EtlChallenge.Contracts.Events.Risk;
using EtlChallenge.Contracts.Events.RiskFile;

namespace EtlChallenge.Application.Handlers;

// Saga state
public class BatchState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public HashSet<string> PoliciesSeen { get; set; } = [];      // policies present
    public HashSet<string> MissingPolicyRefs { get; set; } = []; // risks w/o parent
    public bool PolicyFileDone { get; set; }
    public bool RiskFileDone { get; set; }

    public string PolicyFileReference { get; set; } = string.Empty; // reference to the policy file

    /// <summary>
    /// The saga state machine instance current state
    /// </summary>
    public string CurrentState { get; set; } = string.Empty;
}

public class BatchSaga : MassTransitStateMachine<BatchState>
{
    public Event<PolicyParsedEvent> PolicyParsed { get; } = null!;
    public Event<RiskParsedEvent> RiskParsed { get; } = null!;
    public Event<PolicyFileParseCompletedEvent> PolicyFileParseCompleted { get; } = null!;
    public Event<RiskFileParseCompletedEvent> RiskFileParseCompleted { get; } = null!;

    // TODO: Listen to error events

    public State Active { get; } = null!; // until all files processed

    public BatchSaga(ILogger<BatchSaga> logger)
    {
        InstanceState(x => x.CurrentState);

        Event(() => PolicyParsed, x => x.CorrelateById(context => context.Message.CorrelationId));
        Event(() => RiskParsed, x => x.CorrelateById(context => context.Message.CorrelationId));
        Event(() => PolicyFileParseCompleted, x => x.CorrelateById(context => context.Message.CorrelationId));
        Event(() => RiskFileParseCompleted, x => x.CorrelateById(context => context.Message.CorrelationId));

        SetCompletedWhenFinalized();

        Initially(
            When(PolicyParsed)
                .Then(ctx => logger.LogTrace("Batch process: Policy parsed: {PolicyId}", ctx.Message.Policy.Id))
                .Then(ctx =>
                {
                    HandlePolicy(ctx.Saga, ctx.Message.Policy.Id);
                    ctx.Saga.PolicyFileReference = ctx.Message.PolicyFileReference;
                })
                .TransitionTo(Active),

            When(RiskParsed)
                .Then(ctx => logger.LogTrace("Batch process: Risk parsed: {PolicyId}", ctx.Message.Risk.Id))
                .Then(ctx => HandleRisk(ctx.Saga, ctx.Message.Risk.Id))
                .TransitionTo(Active)
        );

        During(Active,
            When(PolicyParsed)
                .Then(ctx => logger.LogTrace("Batch process: Policy parsed: {PolicyId}", ctx.Message.Policy.Id))
                .Then(ctx =>
                {
                    HandlePolicy(ctx.Saga, ctx.Message.Policy.Id);
                    ctx.Saga.PolicyFileReference = ctx.Message.PolicyFileReference;
                }),

            When(RiskParsed)
                .Then(ctx => logger.LogTrace("Batch process: Risk parsed: {PolicyId}", ctx.Message.Risk.Id))
                .Then(ctx => HandleRisk(ctx.Saga, ctx.Message.Risk.PolicyId)),

            When(PolicyFileParseCompleted)
                .Then(ctx => logger.LogTrace("Batch process: Policy file parse completed: {PolicyFileReference}", ctx.Message.PolicyFileReference))
                .Then(ctx => ctx.Saga.PolicyFileDone = true)
                .ThenAsync(TryFinish)
                .If(ctx => ctx.Saga.PolicyFileDone && ctx.Saga.RiskFileDone,
                    x => x.Finalize()),

            When(RiskFileParseCompleted)
                .Then(ctx => logger.LogTrace("Batch process: Risk file parse completed: {RiskFileReference}", ctx.Message.RiskFileReference))
                .Then(ctx => ctx.Saga.RiskFileDone = true)
                .ThenAsync(TryFinish)
                .If(ctx => ctx.Saga.PolicyFileDone && ctx.Saga.RiskFileDone,
                    x => x.Finalize())
        );
    }

    static void HandlePolicy(BatchState state, string policyId)
    {
        state.PoliciesSeen.Add(policyId);
        state.MissingPolicyRefs.Remove(policyId); // resolve any orphan risks
    }

    static void HandleRisk(BatchState state, string policyId)
    {
        if (!state.PoliciesSeen.Contains(policyId))
            state.MissingPolicyRefs.Add(policyId); // risk before its policy
    }

    static Task TryFinish(BehaviorContext<BatchState> ctx)
    {
        if (ctx.Saga.PolicyFileDone && ctx.Saga.RiskFileDone)
        {
            if (ctx.Saga.MissingPolicyRefs.Count == 0)
                return ctx.Publish(new PolicyFileParseCompletedEvent(ctx.Saga.CorrelationId, ctx.Saga.PolicyFileReference));

            return ctx.Publish(new PolicyFileValidationErrorEvent(
                ctx.Saga.CorrelationId, ctx.Saga.PolicyFileReference,
                [.. ctx.Saga.MissingPolicyRefs]
            ));
        }

        return Task.CompletedTask;
    }
}
