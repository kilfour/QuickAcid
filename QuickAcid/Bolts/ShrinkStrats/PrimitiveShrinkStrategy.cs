using QuickAcid.Bolts.ShrinkStrats;
using QuickAcid.Bolts.TheyCanFade;

namespace QuickAcid.Bolts;

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

    public void Shrink<T>(QAcidState state, string key, T value, string fullKey)
    {
        var actualType = typeof(T) == typeof(object) && value != null
            ? value.GetType()
            : typeof(T);
        var primitiveKey = PrimitiveValues.Keys.FirstOrDefault(k => k.IsAssignableFrom(actualType));
        if (primitiveKey == null)
            return;

        var primitiveVals = PrimitiveValues[primitiveKey];
        var originalFails = state.VerifyIf.RunFailed(key, value!);
        if (!originalFails)
            return;

        foreach (var candidate in primitiveVals.Where(x => !Equals(x, value)))
        {
            if (state.VerifyIf.RunPassed(key, candidate))
            {
                state.GetExecutionContext().Trace(key, ShrinkKind.PrimitiveKind, new ShrinkTrace
                {
                    ExecutionId = -1,
                    Key = fullKey,
                    Name = fullKey.Split(".").Last(),
                    Original = value,
                    Result = candidate,
                    Intent = ShrinkIntent.Keep,
                    Strategy = "PrimitiveShrink"
                });
                return;
            }

            state.GetExecutionContext().Trace(key, ShrinkKind.PrimitiveKind, new ShrinkTrace
            {
                ExecutionId = -1,
                Key = fullKey,
                Name = fullKey.Split(".").Last(),
                Original = value,
                Result = null,
                Intent = ShrinkIntent.Irrelevant,
                Strategy = "PrimitiveShrink"
            });
        }
    }
}
