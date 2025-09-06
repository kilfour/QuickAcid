using QuickAcid.Bolts;
using QuickAcid.Phasers;

namespace QuickAcid.Shrinking.Runners;

public class ExecutionShrinker
{
    public static int Run(QAcidState state)
    {
        var shrinkCount = 0;
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
                if (state.Shifter.CurrentContext.Failed)
                {
                    state.ExecutionNumbers.Remove(current);
                }
                current++;
                shrinkCount++;
            }
        }
        return shrinkCount;
    }
}
