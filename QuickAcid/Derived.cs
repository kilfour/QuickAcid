using QuickAcid.Bolts;
using QuickFuzzr.UnderTheHood;

namespace QuickAcid
{
    public static partial class QAcidCombinators
    {
        public static QAcidScript<T> Derived<T>(this string key, Generator<T> generator) =>
            state => QAcidResult.Some(state, generator(state.FuzzState).Value);

        public static QAcidScript<T> Derived<T>(this string key, Func<T> func) =>
            state => QAcidResult.Some(state, func());
    }
}