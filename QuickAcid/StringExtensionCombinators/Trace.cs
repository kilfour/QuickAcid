using QuickAcid.Bolts;
using QuickAcid.Phasers;
using QuickAcid;

namespace StringExtensionCombinators;

public static partial class QAcidCombinators
{
    public static QAcidScript<string> Trace(this string key, Func<string> trace) =>
        state =>
            state.Shifter.CurrentPhase == QAcidPhase.NormalRun
                ? Vessel.Some(state, state.CurrentExecutionContext().Trace(key, trace()))
                : Vessel.None<string>(state);

    public static QAcidScript<string> TraceIf(this string key, Func<bool> predicate, Func<string> trace) =>
        state => predicate() ? key.Trace(trace)(state) : Vessel.None<string>(state);
}
