using QuickAcid.Bolts;

namespace QuickAcid.ShrinkRunners;

public class InputShrinker
{
    public static int Run(QAcidState state)
    {
        var shrinkCount = 0;
        using (state.EnterPhase(QAcidPhase.ShrinkingInputs))
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
