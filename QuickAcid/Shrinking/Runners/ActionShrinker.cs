using QuickAcid.Bolts;
using QuickAcid.Phasers;
using QuickPulse;
using QuickPulse.Arteries;

namespace QuickAcid.Shrinking.Runners;

public class ActionShrinker
{
    public int Run(QAcidState state)
    {
        var shrinkCount = 0;
        var max = state.ExecutionNumbers.Max();
        foreach (var outerExcutionNumber in state.ExecutionNumbers.ToList())
        {
            var oldKeys = state.Memory.AccessFor(outerExcutionNumber).ActionKeys;
            if (oldKeys.Count < 1) continue;
            foreach (var key in oldKeys.ToList())
            {
                if (oldKeys.Count < 1) continue;
                state.Memory.AccessFor(outerExcutionNumber).ActionKeys.Remove(key);
                using (state.Shifter.EnterPhase(QAcidPhase.ShrinkingExecutions))
                {
                    state.Memory.ResetRunScopedInputs();
                    ExecuteRun(state, key);
                    if (state.Shifter.CurrentContext.Passed)
                    {
                        state.Memory.AccessFor(outerExcutionNumber).ActionKeys.Add(key);
                        OnRunPassed(state, key);
                    }
                    else
                    {
                        OnRunFailed(state, key);
                    }
                    shrinkCount++;
                }
            }
        }
        return shrinkCount;
    }

    protected virtual void ExecuteRun(QAcidState state, string key)
    {
        foreach (var executionNumber in state.ExecutionNumbers.ToList())
        {
            state.CurrentExecutionNumber = executionNumber;
            state.Script(state);
        }
    }

    protected virtual void OnRunPassed(QAcidState state, string key) { }

    protected virtual void OnRunFailed(QAcidState state, string key) { }
}

public class DiagnosticActionShrinker : ActionShrinker
{
    public Signal<string> Signal = QuickPulse.Signal.From<string>(a => Pulse.Trace(a));

    public DiagnosticActionShrinker(IArtery artery) { Signal.SetArtery(artery); }

    protected override void ExecuteRun(QAcidState state, string key)
    {
        Signal.Pulse($"Enter: ActionShrinker.ExecuteRun(state, {key})");
        base.ExecuteRun(state, key);
        Signal.Pulse($"Exit: ActionShrinker.ExecuteRun(state, {key})");
    }

    protected override void OnRunPassed(QAcidState state, string key)
    {
        Signal.Pulse($"Enter: ActionShrinker.OnRunPassed(state, {key})");
        base.OnRunPassed(state, key);
        Signal.Pulse($"Exit: ActionShrinker.OnRunPassed(state, {key})");
    }

    protected override void OnRunFailed(QAcidState state, string key)
    {
        Signal.Pulse($"Enter: ActionShrinker.OnRunFailed(state, {key})");
        base.OnRunFailed(state, key);
        Signal.Pulse($"Exit: ActionShrinker.OnRunFailed(state, {key})");
    }
}
