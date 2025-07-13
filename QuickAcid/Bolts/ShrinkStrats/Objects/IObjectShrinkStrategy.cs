namespace QuickAcid.Bolts.ShrinkStrats.Objects;

public interface IObjectShrinkStrategy
{
    void Shrink<T>(QAcidState state, string key, T value, string fullKey);
}
