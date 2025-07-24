using QuickAcid.Bolts;

namespace QuickAcid.ShrinkRunners;

public class Verifier
{
    private readonly QAcidState state;

    public Verifier(QAcidState state)
    {
        this.state = state;
    }
    public bool RunPassed(string key, object value)
    {
        return !RunFailed(key, value);
    }

    public bool RunFailed(string key, object value)
    {
        using (state.EnterPhase(QAcidPhase.ShrinkInputEval))
        {
            state.Memory.ResetRunScopedInputs();
            var runNumber = state.CurrentExecutionNumber;
            using (state.Memory.ScopedSwap(key, value))
            {

                foreach (var actionNumber in state.ExecutionNumbers)
                {
                    state.CurrentExecutionNumber = actionNumber;
                    state.Script(state);
                }
            }
            state.CurrentExecutionNumber = runNumber;
            return state.CurrentContext.Failed || (state.OriginalRun.Exception == null && state.CurrentContext.Exception != null);
        }
    }
}