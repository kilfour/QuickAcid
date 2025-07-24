using QuickAcid.Bolts;
using QuickAcid.Phasers;

namespace QuickAcid.Shrinking.Runners;

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
        using (state.Shifter.EnterPhase(QAcidPhase.ShrinkInputEval))
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
            return state.Shifter.CurrentContext.Failed
                || (state.Shifter.OriginalRun.Exception == null && state.Shifter.CurrentContext.Exception != null);
        }
    }
}