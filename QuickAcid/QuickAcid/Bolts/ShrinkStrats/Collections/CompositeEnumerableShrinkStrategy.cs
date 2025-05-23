using System.Collections;

namespace QuickAcid.Bolts.ShrinkStrats;

public class EnumerableShrinkStrategy
{
    private readonly ICollectionShrinkPolicy _policy;

    public EnumerableShrinkStrategy(ICollectionShrinkPolicy policy) => _policy = policy;

    public ShrinkOutcome Shrink<T>(QAcidState state, string key, T value, List<string> shrinkValues)
    {
        var list = value;
        foreach (var strategy in _policy.GetStrategies())
        {
            shrinkValues = [];
            list = strategy.Shrink(state, key, list, shrinkValues);
            if (((IList)list).Count == 0)
                return ShrinkOutcome.Irrelevant;
        }
        return ShrinkOutcome.Report($"[ {string.Join(", ", shrinkValues)} ]");
    }
}
