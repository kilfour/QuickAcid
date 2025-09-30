using QuickPulse;
using QuickPulse.Bolts;

namespace QuickAcid.Proceedings.ClerksOffice;


public record Decorum
{
    public Valve Intersperse { get; init; } = Valve.Closed();
    public Valve Line { get; init; } = Valve.Closed();
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

    private readonly static Flow<TraceDeposition> traceDeposition =
        from input in Pulse.Start<TraceDeposition>()
        from _ in newLine
        from __ in Pulse.Trace($"   - Trace: {input.Label.Trim()} = {input.Value}")
        select input;

    private readonly static Flow<InputDeposition> inputDeposition =
        from input in Pulse.Start<InputDeposition>()
        from _ in newLine
        from __ in Pulse.Trace($"   - Input: {input.Label.Trim()} = {input.Value}")
        select input;

    private readonly static Flow<ActionDeposition> actionDeposition =
        from input in Pulse.Start<ActionDeposition>()
        from _ in Pulse.When<Decorum>(a => a.Intersperse.Restricted(), separator)
        from __ in Pulse.Trace($" {input.Label.Trim()}")
        select input;

    private readonly static Flow<TrackedDeposition> trackedDeposition =
        from input in Pulse.Start<TrackedDeposition>()
        from context in Pulse.Gather<Decorum>()
        from _ in Pulse.When<Decorum>(a => a.Line.Passable(), line)
        from __ in Pulse.Trace($"   => {input.Label} (tracked) : {input.Value}")
        from ___ in newLine
        select input;

    private readonly static Flow<ExecutionDeposition> executionDeposition =
        from input in Pulse.Start<ExecutionDeposition>()
        from _ in Pulse.Scoped<Decorum>(
            a => a with { Line = Valve.Install() },
            Pulse.ToFlow(trackedDeposition, input.TrackedDepositions))
        from __ in line
        from ___ in input.Times == 1
                ? Pulse.Trace($"  Executed ({input.ExecutionId}):")
                : Pulse.Trace($"  Executed :")
        from ____ in Pulse.Scoped<Decorum>(
            a => a with { Intersperse = Valve.Install() },
            Pulse.ToFlow(actionDeposition, input.ActionDepositions))
        from _____ in Pulse.TraceIf(input.Times > 1, () => $" ({input.Times} Times)")
        from ______ in Pulse.ToFlow(inputDeposition, input.InputDepositions)
        from _______ in Pulse.ToFlow(traceDeposition, input.TraceDepositions)
        select input;

    private readonly static Flow<ExecutionDeposition> maybeExecutionDeposition =
        from input in Pulse.Start<ExecutionDeposition>()
        from _ in Pulse.ToFlowIf(input.HasContent(), newLine.Then(executionDeposition), () => input)
        select input;

    private readonly static Flow<RunDeposition> runDeposition =
        from input in Pulse.Start<RunDeposition>()
        from _ in line
        from __ in space.Then(Pulse.Trace(input.Label))
        from ___ in Pulse.ToFlow(maybeExecutionDeposition, input.ExecutionDepositions).Then(newLine)
        select input;

    private readonly static Flow<AssayerDeposition> assayerDeposition =
        from input in Pulse.Start<AssayerDeposition>()
        let text = $"  ❌  The Assayer Disagrees: {input.FailedSpec}"
        let length = text.Length + 2
        from _1 in newLine.Then(DoubleLineOf(length)).Then(newLine)
        from _2 in Pulse.Trace(text).Then(newLine)
        from _3 in DoubleLineOf(length)
        select input;

    private readonly static Flow<FailedSpecDeposition> failedSpecDeposition =
        from input in Pulse.Start<FailedSpecDeposition>()
        let text = $"  ❌ Spec Failed: {input.FailedSpec.Trim()}"
        let length = text.Length + 2
        from _1 in newLine.Then(DoubleLineOf(length)).Then(newLine)
        from _2 in Pulse.Trace(text).Then(newLine)
        from _3 in DoubleLineOf(length)
        select input;

    private readonly static Flow<ExceptionDeposition> exceptionDeposition =
        from input in Pulse.Start<ExceptionDeposition>()
        from _1 in newLine.Then(DoubleLineOf(75)).Then(newLine)
        from _2 in Pulse.Trace($"  ❌ Exception Thrown: {input.Exception}").Then(newLine)
        from _3 in DoubleLineOf(75)
        select input;

    private readonly static Flow<FailureDeposition> failureDeposition =
        from input in Pulse.Start<FailureDeposition>()
        from _1 in Pulse.ToFlowIf(input is FailedSpecDeposition, failedSpecDeposition, () => (FailedSpecDeposition)input)
        from _2 in Pulse.ToFlowIf(input is ExceptionDeposition, exceptionDeposition, () => (ExceptionDeposition)input)
        from _3 in Pulse.ToFlowIf(input is AssayerDeposition, assayerDeposition, () => (AssayerDeposition)input)
        select input;

    private readonly static Flow<TestMethodInfoDeposition?> testMethodInfoDeposition =
        from input in Pulse.Start<TestMethodInfoDeposition>()
        from _1 in Pulse.Trace($" Test:                    {input.MethodName}").Then(newLine)
        from _2 in Pulse.Trace($" Location:                {input.SourceFile}:{input.LineNumber}:1").Then(newLine)
        select input;

    private static string Pluralize(int count, string str) =>
        count > 1 ? $"{str}s" : str;

    private readonly static Flow<Verdict?> verdict =
        from verdict in Pulse.Start<Verdict>()
        from _ in line
        from __ in Pulse.ToFlowIf(verdict.TestMethodInfoDeposition != null, testMethodInfoDeposition, () => verdict.TestMethodInfoDeposition)
        let __1 = $"{verdict.OriginalRunExecutionCount} {Pluralize(verdict.OriginalRunExecutionCount, "execution")}"
        from _1 in Pulse.Trace($" Original failing run:    {__1}")
        from n1 in newLine
        let _21 = $"{verdict.ExecutionCount} {Pluralize(verdict.ExecutionCount, "execution")}"
        let _22 = $"{verdict.ShrinkCount} {Pluralize(verdict.ShrinkCount, "shrink")}"
        from _2 in Pulse.TraceIf(verdict.ShrinkCount > 0, () => $" Minimal failing case:    {_21} (after {_22}){Environment.NewLine}")
        from _3 in Pulse.Trace($" Seed:                    {verdict.Seed}")
        from _4 in Pulse.ToFlow(maybeExecutionDeposition, verdict.ExecutionDepositions)
        from _5 in Pulse.ToFlow(failureDeposition, verdict.FailureDeposition)
        select verdict;

    private readonly static Flow<IEnumerable<PassedSpecDeposition>> passedSpecDepositions =
        from input in Pulse.Start<IEnumerable<PassedSpecDeposition>>()
        from _ in Pulse.Trace($" Passed Specs")
        from __ in Pulse.ToFlow(a => Pulse.Trace($"{Environment.NewLine} - {a.Label}: {a.TimesPassed}x"), input)
        from ___ in Pulse.When(input.Any(), newLine.Then(space).Then(LineOf(50)))
        select input;

    private readonly static Flow<DiagnosisDeposition> diagnosisDeposition =
        from input in Pulse.Start<DiagnosisDeposition>()
        from _ in Pulse.Trace($"   - {input.Label}:").Then(newLine)
        from __ in Pulse.ToFlow(a => Pulse.Trace($"     -> {a}").Then(newLine), [.. input.Traces])
        select input;

    private readonly static Flow<DiagnosisExecutionDeposition> diagnosisExecutionDeposition =
        from input in Pulse.Start<DiagnosisExecutionDeposition>()
        from __ in input.Times == 1
                ? Pulse.Trace($"  Diagnosis ({input.ExecutionId}):")
                : Pulse.Trace($"  Diagnosis :")
        from ___ in newLine
        from ____ in Pulse.TraceIf(input.Times > 1, () => $" ({input.Times} Times)")
        from _____ in Pulse.ToFlow(diagnosisDeposition, input.DiagnosisDepositions)
        select input;



    private readonly static Flow<string> extraDepositionMessage =
        from input in Pulse.Start<string>()
        from _ in newLine
        from __ in Pulse.Trace($"  {input}")
        select input;

    public readonly static Flow<CaseFile> CourtStyleGuide =
        from input in Pulse.Start<CaseFile>()
        from _ in Pulse.Gather(new Decorum())
        from runDeposition in Pulse.ToFlow(runDeposition, input.RunDepositions)
        from verdict in Pulse.ToFlowIf(input.Verdict != null, verdict, () => input.Verdict)
        let needsBreak = input.Verdict != null && input.PassedSpecDepositions.Count > 0
        from nl in Pulse.TraceIf(needsBreak, () => Environment.NewLine)
        from __ in Pulse.ToFlowIf(
            input.PassedSpecDepositions.Count > 0,
            passedSpecDepositions,
            () => input.PassedSpecDepositions)
        from ___ in Pulse.ToFlowIf(
            input.DiagnosisExecutionDepositions.Count > 0,
            diagnosisExecutionDeposition,
            () => input.DiagnosisExecutionDepositions)
        select input;
}