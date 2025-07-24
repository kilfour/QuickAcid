using QuickAcid.Bolts;
using QuickAcid.Phasers;

namespace QuickAcid.Shrinking.Runners;

public class InputShrinker
{
    public static int Run(QAcidState state)
    {
        var shrinkCount = 0;
        using (state.Shifter.EnterPhase(QAcidPhase.ShrinkingInputs))
        {
            state.Memory.ResetRunScopedInputs();
            foreach (var executionNumber in state.ExecutionNumbers.ToList())
            {
                state.CurrentExecutionNumber = executionNumber;
                state.Script(state);
                shrinkCount++;
            }
        }
        return shrinkCount;
    }
}
