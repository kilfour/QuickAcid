
using QuickAcid.Bolts;
using QuickAcid.Bolts.Nuts;
using QuickMGenerate;
using QuickMGenerate.UnderTheHood;
using QuickPulse;
using QuickPulse.Arteries;
using QuickPulse.Bolts;

namespace QuickAcid.Tests;

public class Spike
{
    public Generator<int> Counter()
    {
        var counter = 0;
        return
            state =>
                {
                    return new Result<int>(counter++, state);
                };
    }

    private Flow<QAcidState> NormalRun(QAcidScript<Acid> script) =>
        from s in Pulse.Start<QAcidState>()
        from currentExecutionNumber in Pulse.Gather(0)
        from trace in Pulse.Trace(currentExecutionNumber.Value)
        from run in Pulse.Effect(() => script(s))
        from increase in Pulse.Effect(() => currentExecutionNumber.Value++)
        select s;

    [Fact(Skip = "experimenting")]
    public void QuickPulse_for_running_things()
    {
        var script =
            from a in "act".Act(() => { })
            select Acid.Test;

        var flow = NormalRun(script);
        var collector = new TheCollector<int>();
        var signal = Signal.From(flow).SetArtery(collector);
        var state = new QAcidState(null!);

        signal.PulseMultipleUntil(10, () => state.CurrentContext.NeedsToStop(), state);

        Assert.Equal(9, collector.TheExhibit.Last());
    }

    [Fact(Skip = "experimenting")]
    public void QuickPulse_for_running_failing_things()
    {
        var logger = Signal.Tracing<int>().SetArtery(new WriteDataToFile().ClearFile());
        var script =
            from i in "input".Input(Counter())
            from a in "act".Act(() => { logger.Pulse(i); })
            from s in "spec".Spec(() => i != 3)
            select Acid.Test;

        var flow = NormalRun(script);
        var collector = new TheCollector<int>();
        var signal = Signal.From(flow).SetArtery(collector);
        var state = new QAcidState(null!);

        signal.PulseMultipleUntil(10, () => state.CurrentContext.NeedsToStop(), state);

        Assert.Equal(3, collector.TheExhibit.Last());
        Assert.True(state.CurrentContext.Failed);
    }

    [Fact(Skip = "experimenting")]
    public void QuickPulse_for_shrinking_executions()
    {
        var script =
            from i in "input".Input(Counter())
            from a in "act".Act(() => { })
            from s in "spec".Spec(() => i != 3)
            select Acid.Test;

        var flow = NormalRun(script);
        var collector = new TheCollector<int>();
        var signal = Signal.From(flow).SetArtery(collector);
        var state = new QAcidState(null!);

        signal.PulseMultipleUntil(10, () => state.CurrentContext.NeedsToStop(), state);

        var max = collector.TheExhibit.Max();

        var shrinkingExecutions =
            from start in Pulse.Start<(int, int, QAcidState)>()
            from previous in Pulse.Gather(-1)
            let currentExecutionNumber = start.Item1
            let checkingExecution = start.Item2
            let s = start.Item3
            from reset in Pulse.EffectIf(previous.Value != checkingExecution,
                () => { s.Memory.ResetRunScopedInputs(); previous.Value = checkingExecution; })
            from run in Pulse.EffectIf(currentExecutionNumber != checkingExecution, () => script(s))
            from c in Pulse.EffectIf(s.CurrentContext.Failed, () => collector.TheExhibit.Remove(checkingExecution))
            select start;

        var cond =
            from start in Pulse.Start<(int, int, QAcidState)>()
            from sub in Pulse.ToFlowIf(collector.TheExhibit.Count > 1,
                from sub in Pulse.Start<(int, int, QAcidState)>()
                from previous in Pulse.Gather(-1)
                let currentExecutionNumber = start.Item1
                let checkingExecution = start.Item2
                let s = start.Item3
                from reset in Pulse.EffectIf(previous.Value != checkingExecution,
                    () => { s.Memory.ResetRunScopedInputs(); previous.Value = checkingExecution; })
                from run in Pulse.EffectIf(currentExecutionNumber != checkingExecution, () => script(s))
                from c in Pulse.EffectIf(s.CurrentContext.Failed, () => collector.TheExhibit.Remove(checkingExecution))
                select sub,
                    () => start)
            select start;

        var shrinkSignal = Signal.From(cond);
        state.EnterPhase(QAcidPhase.ShrinkingExecutions);
        shrinkSignal.Pulse(collector.TheExhibit.SelectMany(a => collector.TheExhibit.Select(b => (a, b, state))).ToList());

        Assert.Equal(4, collector.TheExhibit.Count); // <= No executions shrunk
        Assert.Equal(3, collector.TheExhibit.Last());
    }

    [Fact(Skip = "experimenting")]
    public void QuickPulse_for_shrinking_executions_needs_shrinking()
    {
        var counter = 0;
        var script =
            from a in "act".Act(() => { if (counter < 3) counter++; })
            from s in "spec".Spec(() => counter != 3)
            select Acid.Test;

        var flow = NormalRun(script);
        var collector = new TheCollector<int>();
        var signal = Signal.From(flow).SetArtery(collector);
        var state = new QAcidState(null!);

        signal.PulseMultipleUntil(10, () => state.CurrentContext.NeedsToStop(), state);

        var max = collector.TheExhibit.Max();

        var shrinkingExecutions =
            from start in Pulse.Start<(int, int, QAcidState)>()
            from previous in Pulse.Gather(-1)
            let currentExecutionNumber = start.Item1
            let checkingExecution = start.Item2
            let s = start.Item3
            from reset in Pulse.EffectIf(previous.Value != checkingExecution,
                () => { s.Memory.ResetRunScopedInputs(); previous.Value = checkingExecution; })
            from run in Pulse.EffectIf(currentExecutionNumber != checkingExecution, () => script(s))
            from c in Pulse.EffectIf(s.CurrentContext.Failed, () => collector.TheExhibit.Remove(checkingExecution))
            select start;

        var cond =
            from start in Pulse.Start<(int, int, QAcidState)>()
            from sub in Pulse.ToFlowIf(collector.TheExhibit.Count > 1,
                from sub in Pulse.Start<(int, int, QAcidState)>()
                from previous in Pulse.Gather(-1)
                let currentExecutionNumber = start.Item1
                let checkingExecution = start.Item2
                let s = start.Item3
                from reset in Pulse.EffectIf(previous.Value != checkingExecution,
                    () => { s.Memory.ResetRunScopedInputs(); previous.Value = checkingExecution; })
                from run in Pulse.EffectIf(currentExecutionNumber != checkingExecution, () => script(s))
                from c in Pulse.EffectIf(s.CurrentContext.Failed, () => collector.TheExhibit.Remove(checkingExecution))
                select sub,
                    () => start)
            select start;

        var shrinkSignal = Signal.From(cond);
        state.EnterPhase(QAcidPhase.ShrinkingExecutions);
        shrinkSignal.Pulse(collector.TheExhibit.SelectMany(a => collector.TheExhibit.Select(b => (a, b, state))).ToList());

        Assert.Single(collector.TheExhibit); // <= 3 executions shrunk
        Assert.Equal(0, collector.TheExhibit.Last()); // does not really matter which one
    }
}