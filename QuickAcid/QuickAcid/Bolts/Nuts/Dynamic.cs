using QuickMGenerate;
using QuickMGenerate.UnderTheHood;

namespace QuickAcid.Bolts.Nuts
{
    public static partial class QAcidCombinators
    {
        public static QAcidRunner<T> Dynamic<T>(this string key, Generator<T> generator)
        {
            return state =>
            {
                var execution = state.GetExecutionContext();
                var value = generator.Generate(); // re-evaluate every execution, always fresh
                return QAcidResult.Some(state, value);
            };
        }
    }
}