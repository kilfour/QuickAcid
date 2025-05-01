using QuickAcid.Bolts.ShrinkStrats;

namespace QuickAcid.Bolts;
public class CustomShrinkStrategy<T> //: IShrinkStrategy
{
    private readonly IShrinker<T> shrinker;

    public CustomShrinkStrategy(IShrinker<T> shrinker)
    {
        this.shrinker = shrinker;
    }
    public ShrinkOutcome Shrink(QState state, string key, T value, Func<object, bool> shrinkingGuard)
    {
        var values = shrinker.Shrink(value);
        var originalFails = state.ShrinkRun(key, value);
        if (!originalFails)
            return ShrinkOutcome.Irrelevant;
        var filtered = values.Where(val => shrinkingGuard(val)).ToArray();
        bool failureAlwaysOccurs =
            filtered
                .Where(x => !Equals(x, value))
                .All(x => state.ShrinkRun(key, x));
        return failureAlwaysOccurs ? ShrinkOutcome.Irrelevant : ShrinkOutcome.Report(QuickAcidStringify.Default()(value));
    }
}