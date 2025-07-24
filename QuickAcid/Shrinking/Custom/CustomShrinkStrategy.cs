using QuickAcid.Bolts;
using QuickPulse.Show;

namespace QuickAcid.Shrinking.Custom;

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
        var originalFails = state.VerifyIf.RunFailed(key, original);
        if (!originalFails)
            return ShrinkOutcome.Irrelevant;

        bool failureAlwaysOccurs =
            values
                .Where(x => !Equals(x, original))
                .All(x => state.VerifyIf.RunFailed(key, x));

        return failureAlwaysOccurs
            ? ShrinkOutcome.Irrelevant
            : ShrinkOutcome.Report(Introduce.This(original, false));
    }
}