using QuickAcid.Bolts;
using QuickAcid;

namespace StringExtensionCombinators;

public static partial class QAcidCombinators
{
    public static QAcidScript<T> Skip<T>(this QAcidScript<T> _) => Vessel.None<T>;
    public static QAcidScript<T> SkipIf<T>(this QAcidScript<T> script, Func<bool> predicate) =>
        state => predicate() ? Vessel.None<T>(state) : script(state);
}
