using QuickAcid.Bolts.ShrinkStrats.Collections;

namespace QuickAcid.Bolts.ShrinkStrats;

public class EnumerableShrinkStrategy
{
    public ShrinkOutcome Shrink<T>(QAcidState state, string key, T value)
    {
        foreach (var strategy in state.GetCollectionStrategies())
        {
            var traces = strategy.Shrink(state, key, value);
            state.Memory.ForThisExecution().GetDecorated(key).ShrinkTraces.AddRange(traces);
        }
        return BuildOutcome(state.Memory.ForThisExecution().GetDecorated(key).ShrinkTraces);
    }

    private ShrinkOutcome BuildOutcome(IEnumerable<ShrinkTrace> traces)
    {
        List<string> shrinkValues = [];
        foreach (var key in traces.Select(a => a.Key).Distinct().Order())
        {
            var tracesForKey = traces.Where(a => a.Key == key);
            if (tracesForKey.Any(a => a.IsRemoved))
                continue;
            if (tracesForKey.Any(a => a.IsIrrelevant))
            {
                shrinkValues.Add("_");
                continue;
            }
            if (tracesForKey.Any(a => a.IsKeep))
            {
                var trace = tracesForKey.First(a => a.IsKeep);
                shrinkValues.Add(QuickAcidStringify.Default()(trace.Original!));
            }
        }
        if (!shrinkValues.Any())
            return ShrinkOutcome.Irrelevant;
        return ShrinkOutcome.Report($"[ {string.Join(", ", shrinkValues)} ]");
    }
}
