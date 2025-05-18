using QuickAcid.Bolts.TheyCanFade;

namespace QuickAcid.Bolts.Nuts;

public static partial class QAcidCombinators
{
    public static QAcidScript<string> Trace(this string key, string info) =>
        state => QAcidResult.Some(state, state.Remember(key, () => info, ReportingIntent.Always));

    public static QAcidScript<string> TraceIf(this string key, Func<bool> predicate, string info) =>
        state => predicate() ? key.Trace(info)(state) : QAcidResult.None<string>(state);
}
