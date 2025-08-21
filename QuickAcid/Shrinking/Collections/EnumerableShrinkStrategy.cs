using QuickAcid.Bolts;
using QuickAcid.TheyCanFade;
using QuickFuzzr;

namespace QuickAcid.Shrinking.Collections;

public class EnumerableShrinkStrategy
{
    public void Shrink<T>(QAcidState state, string key, T value, string fullKey)
    {
        state.CurrentExecutionContext().Trace(key, ShrinkKind.EnumerableKind, new ShrinkTrace
        {
            ExecutionId = -1,
            Key = fullKey,
            Name = fullKey.Split(".").Last(),
            Original = value,
            Result = null,
            Intent = ShrinkIntent.Keep,
            Strategy = "EnumerableShrinkStrategy"
        });
        state.ShrinkingRegistry.GetCollectionStrategies()
            .ForEach(a => { a.Shrink(state, key, value, fullKey); });
    }
}
