namespace QuickAcid.Bolts.ShrinkStrats;

public enum ShrinkIntent
{
    Keep,
    Replace,
    Remove,
    Irrelevant
}

public record ShrinkTrace
{
    public required string Key { get; init; }
    public required string Name { get; init; }
    public required object? Original { get; init; }
    public required object? Result { get; init; }
    public required ShrinkIntent Intent { get; init; }
    public string Strategy { get; init; } = "";

    public bool IsKeep => Intent == ShrinkIntent.Keep;
    public bool IsReplacement => Intent == ShrinkIntent.Replace;
    public bool IsRemoved => Intent == ShrinkIntent.Remove;
    public bool IsIrrelevant => Intent == ShrinkIntent.Irrelevant;

    public static bool HasIrrelevant(IEnumerable<ShrinkTrace> shrinkTraces) => shrinkTraces.Any(a => a.IsIrrelevant);

    public string Report()
    {
        return $"Key:{Key}, Original:{QuickAcidStringify.Default()(Original)}, Strat: {Strategy},Intent:{Intent}.";
    }
}

// public record PrimitiveShrinkTrace : ShrinkTrace
// {

// }

// public record ObjectShrinkTrace : ShrinkTrace
// {
//     //public List<ShrinkTrace> ShrinkTraces { get; init; } = [];


//     // public ShrinkOutcome GetOutcome()
//     // {
//     //     List<string> sValues = [];
//     //     foreach (var key in ShrinkTraces.Select(a => a.Key).Distinct().Order())
//     //     {
//     //         var tracesForKey = ShrinkTraces.Where(a => a.Key == key);
//     //         if (tracesForKey.Any(a => a.IsRemoved))
//     //             continue;
//     //         if (tracesForKey.Any(a => a.IsIrrelevant))
//     //         {
//     //             continue;
//     //         }
//     //         if (tracesForKey.Any(a => a.IsKeep))
//     //         {
//     //             var trace = tracesForKey.First(a => a.IsKeep);
//     //             sValues.Add($"{trace.Name} : {QuickAcidStringify.Default()(trace.Original!)}");
//     //         }
//     //     }
//     //     if (!sValues.Any())
//     //         return ShrinkOutcome.Irrelevant;

//     //     return ShrinkOutcome.Report($"{{ {string.Join(", ", sValues)} }}");
//     // }
// }

// +----------------------------+    +-----------------------------+
// |   Passive Shrinker (e.g.)  |    |  Active Shrinker (e.g.)     |
// | - ObjectShrinkStrategy     |    | - FeedbackPrimitiveShrink   |
// | - ListShrinkStrategy       |    | - FeedbackFusion            |
// +----------------------------+    +-----------------------------+
//           │                                     │
//           │ yields                              │ yields
//           ▼                                     ▼
//   ShrinkTrace:                         ShrinkTrace:
//     Key = "obj.foo"                      Key = "withdraw"
//     Original = 42                        Original = 20
//     Result = null   ← irrelevant         Result = 1000  ← fused replacement
//     Strategy = "RemoveIrrelevant"       Strategy = "FeedbackFusion"

// foreach (var trace in traces)
// {
//     if (trace.IsIrrelevant)
//         // Don't include in final test case
//     else if (trace.IsReplacement)
//         // Inject temporarily during rerun
//     else if (trace.IsKeep)
//         // Retain original input
// }


