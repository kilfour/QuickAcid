using QuickAcid.Bolts;
using QuickFuzzr;

namespace QuickAcid.Shrinking.Objects;

public class CompositeObjectShrinkStrategy
{
    public static void Shrink<T>(QAcidState state, string key, T value, string fullKey)
    {
        state.ShrinkingRegistry.GetObjectStrategies()
            .ToList()
            .ForEach(a =>
            {
                a.Shrink(state, key, value, fullKey);
            });
    }
}
