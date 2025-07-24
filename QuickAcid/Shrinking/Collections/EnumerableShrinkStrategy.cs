using QuickAcid.Bolts;
using QuickAcid.TheyCanFade;
using QuickFuzzr;

namespace QuickAcid.Shrinking.Collections;

public class EnumerableShrinkStrategy
{
    public void Shrink<T>(QAcidState state, string key, T value, string fullKey)
    {
        state.CurrentExecutionContext().SetShrinkKind(key, ShrinkKind.EnumerableKind);
        state.ShrinkingRegistry.GetCollectionStrategies()
            .ForEach(a => { a.Shrink(state, key, value, fullKey); });
    }
}
