using QuickAcid.Bolts.TheyCanFade;
using QuickFuzzr;

namespace QuickAcid.Bolts.ShrinkStrats.Collections;

public class EnumerableShrinkStrategy
{
    public void Shrink<T>(QAcidState state, string key, T value, string fullKey)
    {
        state.ShrinkingRegistry.GetCollectionStrategies()
            .ForEach(a =>
            {
                state.SetShrinkKind(key, ShrinkKind.EnumerableKind);
                // state.Trace(key, ShrinkKind.EnumerableKind, new ShrinkTrace
                // {
                //     Key = fullKey,
                //     Name = fullKey.Split(".").Last(),
                //     Original = value,
                //     Result = null,
                //     Intent = ShrinkIntent.Keep,
                //     Strategy = "EnumerableShrinkStrategy"
                // });
                a.Shrink(state, key, value, fullKey);
            });
    }
}
