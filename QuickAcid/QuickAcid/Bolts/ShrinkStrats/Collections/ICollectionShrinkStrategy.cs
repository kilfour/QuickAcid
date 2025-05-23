namespace QuickAcid.Bolts.ShrinkStrats.Collections;

public interface ICollectionShrinkStrategy
{
    T Shrink<T>(QAcidState state, string key, T value, List<string> shrinkValues);
}
