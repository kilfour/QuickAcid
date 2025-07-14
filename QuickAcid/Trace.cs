using QuickAcid.Bolts;

namespace QuickAcid;

public static partial class QAcidCombinators
{
    public static QAcidScript<string> Trace(this string key, Func<string> trace) =>
        state =>
            state.CurrentPhase == QAcidPhase.NormalRun
                ? QAcidResult.Some(state, state.GetExecutionContext().Trace(key, trace()))
                : QAcidResult.None<string>(state);

    public static QAcidScript<string> TraceIf(this string key, Func<bool> predicate, Func<string> trace) =>
        state => predicate() ? key.Trace(trace)(state) : QAcidResult.None<string>(state);
}
