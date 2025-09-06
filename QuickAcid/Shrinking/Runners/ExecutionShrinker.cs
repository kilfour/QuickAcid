using QuickAcid.Bolts;
using QuickAcid.Phasers;

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
                    foreach (var executionNumber in state.ExecutionNumbers.ToList())
                    {
                        state.CurrentExecutionNumber = executionNumber;
                        if (executionNumber != current)
                            state.Script(state);
                    }
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

    protected virtual void OnRunPassed(QAcidState state, int current) { }

    protected virtual void OnRunFailed(QAcidState state, int current)
        => state.ExecutionNumbers.Remove(current);
}
