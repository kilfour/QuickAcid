using QuickAcid.Bolts;
using QuickAcid.Phasers;
using QuickPulse;
using QuickPulse.Show;

namespace QuickAcid.Shrinking.Runners;

public class Verifier
{
    private readonly QAcidState state;

    public Verifier(QAcidState state)
    {
        this.state = state;
    }

    public virtual bool RunPassed(string key, object value)
    {
        return !RunFailed(key, value);
    }

    public virtual bool RunFailed(string key, object value)
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

public class DiagnosticVerifier : Verifier
{
    public Signal<string> Signal = QuickPulse.Signal.Tracing<string>();

    public DiagnosticVerifier(QAcidState state, IArtery artery)
        : base(state) { Signal.SetArtery(artery); }

    public override bool RunPassed(string key, object value)
    {
        Signal.Pulse($"Enter: Verifier.RunPassed({key}, {Introduce.This(value, false)})");
        var result = base.RunPassed(key, value);
        Signal.Pulse($"    Result: {result})");
        return result;
    }

    public override bool RunFailed(string key, object value)
    {
        Signal.Pulse($"Enter: Verifier.RunFailed({key}, {Introduce.This(value, false)})");
        var result = base.RunFailed(key, value);
        Signal.Pulse($"    Result: {result})");
        return result;
    }
}