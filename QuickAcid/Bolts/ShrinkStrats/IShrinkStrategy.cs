namespace QuickAcid.Bolts.ShrinkStrats;

public interface IShrinkStrategy
{
    ShrinkOutcome Shrink<T>(QAcidState state, string key, T value, Func<object, bool> shrinkingGuard);
}
