using QuickAcid.Bolts.ShrinkStrats;
using QuickPulse.Show;

namespace QuickAcid.Bolts;

public class CustomShrinkStrategy
{
    private readonly IShrinkerBox shrinker;

    public CustomShrinkStrategy(IShrinkerBox shrinker)
    {
        this.shrinker = shrinker;
    }

    public ShrinkOutcome Shrink(QAcidState state, string key, object original)
    {
        var values = shrinker.Shrink(original);
        var originalFails = state.RunFailed(key, original);
        if (!originalFails)
            return ShrinkOutcome.Irrelevant;

        bool failureAlwaysOccurs =
            values
                .Where(x => !Equals(x, original))
                .All(x => state.RunFailed(key, x));

        return failureAlwaysOccurs
            ? ShrinkOutcome.Irrelevant
            : ShrinkOutcome.Report(Introduce.This(original, false));
    }
}