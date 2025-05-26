using QuickAcid.Bolts.ShrinkStrats;

namespace QuickAcid.Bolts;

// public class PrimitiveShrinkStrategy //: IShrinkStrategy
// {
//     public static readonly Dictionary<Type, object[]> PrimitiveValues =
//         new()
//         {
//             { typeof(bool),     new object[] { true, false } },
//             { typeof(byte),     new object[] { (byte)0, (byte)1, byte.MaxValue } },
//             { typeof(sbyte),    new object[] { sbyte.MinValue, -1, 0, 1, sbyte.MaxValue } },
//             { typeof(short),    new object[] { short.MinValue, -1, 0, 1, short.MaxValue } },
//             { typeof(ushort),   new object[] { (ushort)0, (ushort)1, ushort.MaxValue } },
//             { typeof(int),      new object[] { -10000, -1000, -1, 0, 1, 1000, 10000 } },
//             { typeof(uint),     new object[] { 0u, 1u, 1000u, uint.MaxValue } },
//             { typeof(long),     new object[] { long.MinValue, -1000L, -1L, 0L, 1L, 1000L, long.MaxValue } },
//             { typeof(ulong),    new object[] { 0UL, 1UL, 1000UL, ulong.MaxValue } },
//             { typeof(float),    new object[] { float.MinValue, -1f, 0f, 1f, float.MaxValue, float.NaN, float.NegativeInfinity, float.PositiveInfinity } },
//             { typeof(double),   new object[] { double.MinValue, -1.0, 0.0, 1.0, double.MaxValue, double.NaN, double.NegativeInfinity, double.PositiveInfinity } },
//             { typeof(decimal),  new object[] { decimal.MinValue, -1m, 0m, 1m, decimal.MaxValue } },
//             { typeof(char),     new object[] { '\0', 'a', 'Z', ' ', '\n', '\uFFFF' } },
//             { typeof(string),   new object[] { null!, "", "a", "the quick brown fox", new string('-', 256), new string('-', 1024) } },
//             { typeof(DateTime), new object[] { DateTime.MinValue, DateTime.MaxValue, DateTime.UnixEpoch, DateTime.UtcNow, DateTime.Today } },
//             { typeof(DateTimeOffset), new object[] { DateTimeOffset.MinValue, DateTimeOffset.MaxValue, DateTimeOffset.UtcNow, DateTimeOffset.Now } },
//             { typeof(TimeSpan), new object[] { TimeSpan.Zero, TimeSpan.FromSeconds(1), TimeSpan.FromDays(1), TimeSpan.MinValue, TimeSpan.MaxValue } },
//             { typeof(Guid),     new object[] { Guid.Empty, Guid.NewGuid() } },
//             { typeof(Uri),      new object[] { null!, new Uri("http://localhost"), new Uri("https://example.com/resource?query=1") } }
//         };

//     public ShrinkOutcome Shrink<T>(QAcidState state, string key, T value)
//     {
//         var actualType = typeof(T) == typeof(object) && value != null
//             ? value.GetType()
//             : typeof(T);
//         var primitiveKey = PrimitiveValues.Keys.FirstOrDefault(k => k.IsAssignableFrom(actualType));
//         var primitiveVals = PrimitiveValues[primitiveKey!];
//         var originalFails = state.ShrinkRunReturnTrueIfFailed(key, value!);
//         if (!originalFails)
//             return ShrinkOutcome.Irrelevant;

//         if (state.InFeedbackShrinkingPhase)
//         {
//             foreach (object val in primitiveVals.Where(x => !Equals(x, value)))
//             {
//                 if (state.ShrinkRunReturnTrueIfFailed(key, val))
//                 {
//                     state.Memory.ForThisExecution().Override(key, val);
//                     return ShrinkOutcome.Report(QuickAcidStringify.Default()(val));
//                 }
//             }
//             return ShrinkOutcome.Report(QuickAcidStringify.Default()(value!));
//         }
//         else
//         {
//             foreach (object val in primitiveVals.Where(x => !Equals(x, value)))
//             {
//                 if (!state.ShrinkRunReturnTrueIfFailed(key, val))
//                 {
//                     return ShrinkOutcome.Report(QuickAcidStringify.Default()(value!));
//                 }
//             }
//             return ShrinkOutcome.Irrelevant;
//         }
//     }
// }


public class PrimitiveShrinkStrategy
{
    public static readonly Dictionary<Type, object[]> PrimitiveValues =
        new()
        {
            { typeof(bool),     new object[] { true, false } },
            { typeof(byte),     new object[] { (byte)0, (byte)1, byte.MaxValue } },
            { typeof(sbyte),    new object[] { sbyte.MinValue, -1, 0, 1, sbyte.MaxValue } },
            { typeof(short),    new object[] { short.MinValue, -1, 0, 1, short.MaxValue } },
            { typeof(ushort),   new object[] { (ushort)0, (ushort)1, ushort.MaxValue } },
            { typeof(int),      new object[] { -10000, -1000, -1, 0, 1, 1000, 10000 } },
            { typeof(uint),     new object[] { 0u, 1u, 1000u, uint.MaxValue } },
            { typeof(long),     new object[] { long.MinValue, -1000L, -1L, 0L, 1L, 1000L, long.MaxValue } },
            { typeof(ulong),    new object[] { 0UL, 1UL, 1000UL, ulong.MaxValue } },
            { typeof(float),    new object[] { float.MinValue, -1f, 0f, 1f, float.MaxValue, float.NaN, float.NegativeInfinity, float.PositiveInfinity } },
            { typeof(double),   new object[] { double.MinValue, -1.0, 0.0, 1.0, double.MaxValue, double.NaN, double.NegativeInfinity, double.PositiveInfinity } },
            { typeof(decimal),  new object[] { decimal.MinValue, -1m, 0m, 1m, decimal.MaxValue } },
            { typeof(char),     new object[] { '\0', 'a', 'Z', ' ', '\n', '\uFFFF' } },
            { typeof(string),   new object[] { null!, "", "a", "the quick brown fox", new string('-', 256), new string('-', 1024) } },
            { typeof(DateTime), new object[] { DateTime.MinValue, DateTime.MaxValue, DateTime.UnixEpoch, DateTime.UtcNow, DateTime.Today } },
            { typeof(DateTimeOffset), new object[] { DateTimeOffset.MinValue, DateTimeOffset.MaxValue, DateTimeOffset.UtcNow, DateTimeOffset.Now } },
            { typeof(TimeSpan), new object[] { TimeSpan.Zero, TimeSpan.FromSeconds(1), TimeSpan.FromDays(1), TimeSpan.MinValue, TimeSpan.MaxValue } },
            { typeof(Guid),     new object[] { Guid.Empty, Guid.NewGuid() } },
            { typeof(Uri),      new object[] { null!, new Uri("http://localhost"), new Uri("https://example.com/resource?query=1") } }
        };

    public IEnumerable<ShrinkTrace> Shrink<T>(QAcidState state, string key, T value)
    {
        var actualType = typeof(T) == typeof(object) && value != null
            ? value.GetType()
            : typeof(T);
        var primitiveKey = PrimitiveValues.Keys.FirstOrDefault(k => k.IsAssignableFrom(actualType));
        if (primitiveKey == null)
            yield break;

        var primitiveVals = PrimitiveValues[primitiveKey];
        var originalFails = state.ShrinkRunReturnTrueIfFailed(key, value);
        if (!originalFails)
            yield break;

        var stringify = QuickAcidStringify.Default();

        if (state.InFeedbackShrinkingPhase)
        {
            foreach (var candidate in primitiveVals.Where(x => !Equals(x, value)))
            {
                if (state.ShrinkRunReturnTrueIfFailed(key, candidate))
                {
                    yield return new ShrinkTrace
                    {
                        Key = key,
                        Original = value,
                        Result = candidate,
                        Intent = ShrinkIntent.Replace,
                        Strategy = "PrimitiveShrink.FeedbackFusion",
                        Message = $"Replaced with {stringify(candidate)} to retain failure in feedback phase"
                    };
                    yield break;
                }
            }

            yield return new ShrinkTrace
            {
                Key = key,
                Original = value,
                Result = value,
                Intent = ShrinkIntent.Keep,
                Strategy = "PrimitiveShrink.FeedbackFusion",
                Message = $"No better replacement found"
            };
        }
        else
        {
            foreach (var candidate in primitiveVals.Where(x => !Equals(x, value)))
            {
                if (!state.ShrinkRunReturnTrueIfFailed(key, candidate))
                {
                    // This candidate caused test to pass — so we can't shrink past this value
                    yield return new ShrinkTrace
                    {
                        Key = key,
                        Original = value,
                        Result = value,
                        Intent = ShrinkIntent.Keep,
                        Strategy = "PrimitiveShrink",
                        Message = $"Minimal value causing failure"
                    };
                    yield break;
                }
            }

            // If all alternates failed just like the original → this input didn't matter
            yield return new ShrinkTrace
            {
                Key = key,
                Original = value,
                Result = null,
                Intent = ShrinkIntent.Irrelevant,
                Strategy = "PrimitiveShrink",
                Message = "Input value is irrelevant to failure"
            };
        }
    }
}
