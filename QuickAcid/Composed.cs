using QuickAcid.Bolts;
using QuickMGenerate;
using QuickMGenerate.UnderTheHood;

namespace QuickAcid
{
    public static partial class QAcidCombinators
    {

        public static QAcidScript<T> Composed<T>(this string key, QAcidScript<T> script)
        {
            return state =>
            {
                var execution = state.GetExecutionContext();
                return script(state);
                //var value = generator.Generate();
                //return QAcidResult.Some(state, value);
                //return QAcidResult.None<T>(state);
            };
        }
    }
}