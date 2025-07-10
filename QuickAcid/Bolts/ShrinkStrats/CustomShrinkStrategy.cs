using QuickAcid.Bolts.ShrinkStrats;

namespace QuickAcid.Bolts;

public class CustomShrinkStrategy<T>
{
    private readonly IShrinker<T> shrinker;

    public CustomShrinkStrategy(IShrinker<T> shrinker)
    {
        this.shrinker = shrinker;
    }
    public ShrinkOutcome Shrink(QAcidState state, string key, T value)
    {
        var values = shrinker.Shrink(value);
        var originalFails = state.ShrinkRunReturnTrueIfFailed(key, value!);
        if (!originalFails)
            return ShrinkOutcome.Irrelevant;
        var filtered = values;
        bool failureAlwaysOccurs =
            filtered
                .Where(x => !Equals(x, value))
                .All(x => state.ShrinkRunReturnTrueIfFailed(key, x!));
        return failureAlwaysOccurs ? ShrinkOutcome.Irrelevant : ShrinkOutcome.Report(QuickAcidStringify.Default()(value!));
    }
}