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
    public required object? Original { get; init; }
    public required object? Result { get; init; }
    public required ShrinkIntent Intent { get; init; }
    public string Strategy { get; init; } = "";
    public string? Message { get; init; }

    public bool IsKeep => Intent == ShrinkIntent.Keep;
    public bool IsReplacement => Intent == ShrinkIntent.Replace;
    public bool IsRemoved => Intent == ShrinkIntent.Remove;
    public bool IsIrrelevant => Intent == ShrinkIntent.Irrelevant;

    public static bool HasIrrelevant(IEnumerable<ShrinkTrace> shrinkTraces) => shrinkTraces.Any(a => a.IsIrrelevant);
}

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


