using QuickAcid.Bolts;
using QuickAcid.TheyCanFade;
using QuickPulse.Show;

namespace QuickAcid.Shrinking.Custom;

public class CustomShrinkStrategy
{
    private readonly IShrinkerBox shrinker;

    public CustomShrinkStrategy(IShrinkerBox shrinker)
    {
        this.shrinker = shrinker;
    }

    public void Shrink(QAcidState state, string key, object original)
    {
        var values = shrinker.Shrink(original);
        var originalFails = state.VerifyIf.RunFailed(key, original);
        if (!originalFails)
            return;

        foreach (var candidate in values.Where(x => !Equals(x, original)))
        {
            if (state.VerifyIf.RunPassed(key, candidate))
            {
                state.CurrentExecutionContext().ShrinkTrace(key, ShrinkKind.PrimitiveKind, new ShrinkTrace
                {
                    ExecutionId = -1,
                    Key = key,
                    Name = key.Split(".").Last(),
                    Original = original,
                    Result = candidate,
                    Intent = ShrinkIntent.Keep,
                    Strategy = "CustomShrink"
                });
                return;
            }
        }
        state.CurrentExecutionContext().ShrinkTrace(key, ShrinkKind.PrimitiveKind, new ShrinkTrace
        {
            ExecutionId = -1,
            Key = key,
            Name = key.Split(".").Last(),
            Original = original,
            Result = null,
            Intent = ShrinkIntent.Irrelevant,
            Strategy = "CustomShrink"
        });
    }
}