using QuickAcid.Bolts;

namespace QuickAcid.ShrinkRunners;

public class ActionShrinker
{
    public static int Run(QAcidState state)
    {
        var shrinkCount = 0;
        var max = state.ExecutionNumbers.Max();
        foreach (var outerExcutionNumber in state.ExecutionNumbers.ToList())
        {
            var oldKeys = state.Memory.For(outerExcutionNumber).ActionKeys;
            if (oldKeys.Count < 1) continue;
            foreach (var key in oldKeys.ToList())
            {
                if (oldKeys.Count < 1) continue;
                state.Memory.For(outerExcutionNumber).ActionKeys.Remove(key);
                using (state.EnterPhase(QAcidPhase.ShrinkingExecutions))
                {
                    state.Memory.ResetRunScopedInputs();
                    foreach (var executionNumber in state.ExecutionNumbers.ToList())
                    {
                        state.CurrentExecutionNumber = executionNumber;
                        state.Script(state);
                    }
                    if (!state.CurrentContext.Failed)
                    {
                        state.Memory.For(outerExcutionNumber).ActionKeys.Add(key);
                    }
                    shrinkCount++;
                }
            }
        }
        return shrinkCount;
    }
}
