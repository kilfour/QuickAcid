using System.Reflection.Metadata.Ecma335;
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

    public int Level { get; init; } = 0;
    public FlowContext IncreaseLevel()
    {
        return this with { Level = Level + 1 };
    }
}

public static class The
{
    private readonly static Flow<Unit> separator = Pulse.Trace(",");

    private readonly static Flow<Unit> space = Pulse.Trace(" ");

    private static readonly Flow<Unit> newLine = Pulse.Trace(Environment.NewLine);

    private static Flow<Unit> LineOf(int length) => Pulse.Trace(new string('─', length));

    private static Flow<Unit> DoubleLineOf(int length) =>
        space.Then(Pulse.Trace(new string('═', length)));

    private readonly static Flow<Unit> line =
        space.Then(LineOf(50)).Then(newLine);

    private readonly static Flow<Unit> longerLine =
         space.Then(LineOf(75));

    private readonly static Flow<InputDeposition> inputDeposition =
        from input in Pulse.Start<InputDeposition>()
        from _ in newLine
        from __ in Pulse.Trace($"   - Input: {input.InputLabel} = {Introduce.This(input.InputValue, false)}")
        select input;

    private readonly static Flow<ActionDeposition> actionDeposition =
        from input in Pulse.Start<ActionDeposition>()
        from context in Pulse.Gather<FlowContext>()
        from seperator in Pulse.When(!context.Value.Intersperse.Spring(), separator)
        from _ in Pulse.Trace($" {input.ActionLabel}")
        select input;

    private readonly static Flow<ExecutionDeposition> executionDeposition =
        from input in Pulse.Start<ExecutionDeposition>()
        from _ in line
        from __ in Pulse.Trace($" Executed ({input.ExecutionId}):")
        from ___ in Pulse.Scoped<FlowContext>(a => a.PrimeIntersperse(), Pulse.ToFlow(actionDeposition, input.ActionDepositions))
        from ____ in Pulse.ToFlow(inputDeposition, input.InputDepositions)
        select input;

    private readonly static Flow<ExecutionDeposition> maybeExecutionDeposition =
        from input in Pulse.Start<ExecutionDeposition>()
        from _ in Pulse.ToFlowIf(input.HasContent(), newLine.Then(executionDeposition), () => input)
        select input;

    private readonly static Flow<RunDeposition> runDeposition =
        from input in Pulse.Start<RunDeposition>()
        from _ in Pulse.ToFlow(maybeExecutionDeposition, input.ExecutionDepositions)
        select input;

    private readonly static Flow<FailedSpecDeposition> failedSpecDeposition =
        from input in Pulse.Start<FailedSpecDeposition>()
        let text = $"  ❌ Spec Failed: {input.FailedSpec}"
        let length = text.Length + 2
        from _1 in newLine.Then(DoubleLineOf(length)).Then(newLine)
        from _2 in Pulse.Trace(text).Then(newLine)
        from _3 in DoubleLineOf(length)
        select input;

    private readonly static Flow<ExceptionDeposition> exceptionDeposition =
        from input in Pulse.Start<ExceptionDeposition>()
        from _1 in newLine.Then(longerLine).Then(newLine)
        from _2 in Pulse.Trace($"  ❌ Exception Thrown: {input.Exception}").Then(newLine)
        from _3 in longerLine
        select input;

    private readonly static Flow<FailureDeposition> failureDeposition =
        from input in Pulse.Start<FailureDeposition>()
        from _1 in Pulse.ToFlowIf(input is FailedSpecDeposition, failedSpecDeposition, () => (FailedSpecDeposition)input)
        from _2 in Pulse.ToFlowIf(input is ExceptionDeposition, exceptionDeposition, () => (ExceptionDeposition)input)
        select input;

    private static string Pluralize(int count, string str) =>
        count > 1 ? $"{str}s" : str;

    private readonly static Flow<Verdict?> verdict =
        from verdict in Pulse.Start<Verdict>()
        from _ in line
        let __1 = $"{verdict.OriginalRunExecutionCount} {Pluralize(verdict.OriginalRunExecutionCount, "execution")}"
        from _1 in Pulse.Trace($" Original failing run:    {__1}.")
        from n1 in newLine
        let _21 = $"{verdict.ExecutionCount} {Pluralize(verdict.ExecutionCount, "execution")}"
        let _22 = $"{verdict.ShrinkCount} {Pluralize(verdict.ShrinkCount, "shrink")}"
        from _2 in Pulse.Trace($" Minimal failing case:    {_21} after ({_22}).")
        from n2 in newLine
        from _3 in Pulse.Trace($" Seed:                    {verdict.Seed}.")
        from _4 in Pulse.ToFlow(maybeExecutionDeposition, verdict.ExecutionDepositions)
        from _5 in Pulse.ToFlow(failureDeposition, verdict.FailureDeposition)
        select verdict;

    public readonly static Flow<CaseFile> CourtStyleGuide =
        from input in Pulse.Start<CaseFile>()
        from _ in Pulse.Gather(new FlowContext())
        from runDeposition in Pulse.ToFlow(runDeposition, input.RunDepositions)
        from verdict in Pulse.ToFlow(verdict, input.Verdict)
        select input;
}