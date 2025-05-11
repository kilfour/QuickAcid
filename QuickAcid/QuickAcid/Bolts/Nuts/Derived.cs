using QuickMGenerate;
using QuickMGenerate.UnderTheHood;

namespace QuickAcid.Bolts.Nuts
{
    public static partial class QAcidCombinators
    {
        public static QAcidScript<T> Derived<T>(this string key, Generator<T> generator)
        {
            return state =>
            {
                var execution = state.GetExecutionContext();
                var value = generator.Generate();
                return QAcidResult.Some(state, value);
            };
        }
    }
}