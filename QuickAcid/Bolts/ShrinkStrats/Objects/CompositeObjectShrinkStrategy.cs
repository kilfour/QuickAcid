using QuickMGenerate;

namespace QuickAcid.Bolts.ShrinkStrats.Objects;

public class CompositeObjectShrinkStrategy
{
    public void Shrink<T>(QAcidState state, string key, T value, string fullKey)
    {
        state.GetObjectStrategies()
            .ForEach(a =>
            {
                a.Shrink(state, key, value, fullKey);
            });
    }
}
