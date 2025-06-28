using QuickAcid.Bolts;

namespace QuickAcid;

public static partial class QAcidCombinators
{
    public static QAcidScript<T> Skip<T>(this QAcidScript<T> _) => QAcidResult.None<T>;
    public static QAcidScript<T> SkipIf<T>(this QAcidScript<T> script, Func<bool> predicate) =>
        state => predicate() ? QAcidResult.None<T>(state) : script(state);
}
