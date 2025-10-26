using QuickAcid.Bolts;
using QuickAcid.Phasers;
using QuickPulse;
using QuickPulse.Arteries;

namespace QuickAcid.Shrinking.Runners;

public class ExecutionShrinker
{
    public int Run(QAcidState state)
    {
        var shrinkCount = 0;
        var stillShrinking = true;
        while (stillShrinking) // not sure why this is needed
        {
            stillShrinking = false;
            var max = state.ExecutionNumbers.Max();
            var current = 0;
            while (current <= max && state.ExecutionNumbers.Count > 1)
            {
                using (state.Shifter.EnterPhase(QAcidPhase.ShrinkingExecutions))
                {
                    state.Memory.ResetRunScopedInputs();
                    ExecuteRun(state, current);
                    if (state.Shifter.CurrentContext.Passed)
                        OnRunPassed(state, current);
                    else
                    {
                        if (state.ExecutionNumbers.Contains(current)) stillShrinking = true;
                        OnRunFailed(state, current);
                    }
                    current++;
                    shrinkCount++;
                }
            }
        }
        return shrinkCount;
    }

    protected virtual void ExecuteRun(QAcidState state, int current)
    {
        foreach (var executionNumber in state.ExecutionNumbers.ToList())
        {
            state.CurrentExecutionNumber = executionNumber;
            if (executionNumber != current)
                state.Script(state);
        }
    }

    protected virtual void OnRunPassed(QAcidState state, int current) { }

    protected virtual void OnRunFailed(QAcidState state, int current)
        => state.ExecutionNumbers.Remove(current);
}

public class DiagnosticExecutionShrinker : ExecutionShrinker
{
    public Signal<string> Signal = QuickPulse.Signal.From<string>(a => Pulse.Trace(a));

    public DiagnosticExecutionShrinker(IArtery artery) { Signal.SetArtery(artery); }

    protected override void ExecuteRun(QAcidState state, int current)
    {
        Signal.Pulse($"Enter: ExecutionShrinker.ExecuteRun(state, {current})");
        base.ExecuteRun(state, current);
        Signal.Pulse($"Exit: ExecutionShrinker.ExecuteRun(state, {current})");
    }

    protected override void OnRunPassed(QAcidState state, int current)
    {
        Signal.Pulse($"Enter: ExecutionShrinker.OnRunPassed(state, {current})");
        base.OnRunPassed(state, current);
        Signal.Pulse($"Exit: ExecutionShrinker.OnRunPassed(state, {current})");
    }

    protected override void OnRunFailed(QAcidState state, int current)
    {
        Signal.Pulse($"Enter: ExecutionShrinker.OnRunFailed(state, {current})");
        base.OnRunFailed(state, current);
        Signal.Pulse($"Exit: ExecutionShrinker.OnRunFailed(state, {current})");
    }
}