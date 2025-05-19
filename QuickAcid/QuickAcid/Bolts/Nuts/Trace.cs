using QuickAcid.Bolts.TheyCanFade;

namespace QuickAcid.Bolts.Nuts;

public static partial class QAcidCombinators
{
    public static QAcidScript<string> Trace(this string key, string trace) =>
        state => QAcidResult.Some(state, state.GetExecutionContext().Trace(key, trace));

    public static QAcidScript<string> TraceIf(this string key, Func<bool> predicate, string trace) =>
        state => predicate() ? key.Trace(trace)(state) : QAcidResult.None<string>(state);
}
