namespace QuickAcid.Bolts.ShrinkStrats.Collections;

public interface ICollectionShrinkStrategy
{
    IEnumerable<ShrinkTrace> Shrink<T>(QAcidState state, string key, T value);
}
