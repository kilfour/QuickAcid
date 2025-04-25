namespace QuickAcid.Bolts;

public interface IShrinkStrategy
{
    ShrinkOutcome Shrink<T>(QAcidState state, string key, T value, Func<object, bool> shrinkingGuard);
}
