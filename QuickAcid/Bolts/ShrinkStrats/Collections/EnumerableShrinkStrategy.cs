using QuickAcid.Bolts.TheyCanFade;
using QuickFuzzr;

namespace QuickAcid.Bolts.ShrinkStrats.Collections;

public class EnumerableShrinkStrategy
{
    public void Shrink<T>(QAcidState state, string key, T value, string fullKey)
    {
        state.GetExecutionContext().SetShrinkKind(key, ShrinkKind.EnumerableKind);
        state.ShrinkingRegistry.GetCollectionStrategies()
            .ForEach(a => { a.Shrink(state, key, value, fullKey); });
    }
}
