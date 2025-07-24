using QuickAcid.Bolts;

namespace QuickAcid.Shrinking.Collections;

public interface ICollectionShrinkStrategy
{
    void Shrink<T>(QAcidState state, string key, T value, string fullKey);
}
