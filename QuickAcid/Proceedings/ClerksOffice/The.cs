using QuickPulse;
using QuickPulse.Bolts;
using QuickPulse.Show;

namespace QuickAcid.Proceedings.ClerksOffice;

public record Trap
{
    private bool primed;
    public Trap(bool primed) { this.primed = primed; }
    //public void Prime() { primed = true; }
    public bool Spring() { var result = primed; primed = false; return result; }
}

public record FlowContext
{
    public Trap Intersperse { get; init; } = new Trap(false);

    public FlowContext PrimeIntersperse()
    {
        return this with { Intersperse = new Trap(true) };
    }
}

public static class The
{
    private static Flow<Unit> NewLine = Pulse.Trace(Environment.NewLine);

    private readonly static Flow<Unit> Separator = Pulse.Trace(",");

    private readonly static Flow<InputDeposition> inputDeposition =
        from input in Pulse.Start<InputDeposition>()
        from _ in NewLine
        from __ in Pulse.Trace($"   - Input: {input.InputLabel} = {Introduce.This(input.InputValue, false)}")
        select input;

    private readonly static Flow<ActionDeposition> actionDeposition =
        from input in Pulse.Start<ActionDeposition>()
        from context in Pulse.Gather<FlowContext>()
        from seperator in Pulse.When(!context.Value.Intersperse.Spring(), Separator)
        from _ in Pulse.Trace($" {input.ActionLabel}")
        select input;

    private readonly static Flow<ExecutionDeposition> executionDeposition =
        from input in Pulse.Start<ExecutionDeposition>()
        from _ in Pulse.Trace(" ──────────────────────────────────────────────────")
        from __ in NewLine
        from ___ in Pulse.Trace($" Executed ({input.ExecutionId}):")
        from ____ in Pulse.Scoped<FlowContext>(a => a.PrimeIntersperse(), Pulse.ToFlow(actionDeposition, input.ActionDepositions))
        from _____ in Pulse.ToFlow(inputDeposition, input.InputDepositions)
        select input;

    private readonly static Flow<ExecutionDeposition> maybeExecutionDeposition =
        from input in Pulse.Start<ExecutionDeposition>()
        from _ in Pulse.ToFlowIf(input.HasContent(), executionDeposition, () => input)
        select input;

    public readonly static Flow<CaseFile> CourtStyleGuide =
        from input in Pulse.Start<CaseFile>()
        from _ in Pulse.Gather(new FlowContext())
        from executions in Pulse.ToFlow(maybeExecutionDeposition, input.ExecutionDepositions)
        select input;
}