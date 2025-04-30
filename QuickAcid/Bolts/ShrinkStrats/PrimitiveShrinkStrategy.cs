using QuickAcid.Bolts.ShrinkStrats;

namespace QuickAcid.Bolts;
public class PrimitiveShrinkStrategy : IShrinkStrategy
{
    public static readonly Dictionary<Type, object[]> PrimitiveValues =
        new()
        {
            { typeof(bool),     new object[] { true, false } },
            { typeof(byte),     new object[] { (byte)0, (byte)1, byte.MaxValue } },
            { typeof(sbyte),    new object[] { sbyte.MinValue, -1, 0, 1, sbyte.MaxValue } },
            { typeof(short),    new object[] { short.MinValue, -1, 0, 1, short.MaxValue } },
            { typeof(ushort),   new object[] { (ushort)0, (ushort)1, ushort.MaxValue } },
            { typeof(int),      new object[] { int.MinValue, -1000, -1, 0, 1, 1000, int.MaxValue } },
            { typeof(uint),     new object[] { 0u, 1u, 1000u, uint.MaxValue } },
            { typeof(long),     new object[] { long.MinValue, -1000L, -1L, 0L, 1L, 1000L, long.MaxValue } },
            { typeof(ulong),    new object[] { 0UL, 1UL, 1000UL, ulong.MaxValue } },
            { typeof(float),    new object[] { float.MinValue, -1f, 0f, 1f, float.MaxValue, float.NaN, float.NegativeInfinity, float.PositiveInfinity } },
            { typeof(double),   new object[] { double.MinValue, -1.0, 0.0, 1.0, double.MaxValue, double.NaN, double.NegativeInfinity, double.PositiveInfinity } },
            { typeof(decimal),  new object[] { decimal.MinValue, -1m, 0m, 1m, decimal.MaxValue } },
            { typeof(char),     new object[] { '\0', 'a', 'Z', ' ', '\n', '\uFFFF' } },
            { typeof(string),   new object[] { null, "", "a", "the quick brown fox", new string('-', 256), new string('-', 1024) } },
            { typeof(DateTime), new object[] { DateTime.MinValue, DateTime.MaxValue, DateTime.UnixEpoch, DateTime.UtcNow, DateTime.Today } },
            { typeof(DateTimeOffset), new object[] { DateTimeOffset.MinValue, DateTimeOffset.MaxValue, DateTimeOffset.UtcNow, DateTimeOffset.Now } },
            { typeof(TimeSpan), new object[] { TimeSpan.Zero, TimeSpan.FromSeconds(1), TimeSpan.FromDays(1), TimeSpan.MinValue, TimeSpan.MaxValue } },
            { typeof(Guid),     new object[] { Guid.Empty, Guid.NewGuid() } },
            { typeof(Uri),      new object[] { null, new Uri("http://localhost"), new Uri("https://example.com/resource?query=1") } }
        };

    public ShrinkOutcome Shrink<T>(QAcidState state, string key, T value, Func<object, bool> shrinkingGuard)
    {
        var actualType = typeof(T) == typeof(object) && value != null
            ? value.GetType()
            : typeof(T);
        var primitiveKey = PrimitiveValues.Keys.FirstOrDefault(k => k.IsAssignableFrom(actualType));
        var primitiveVals = PrimitiveValues[primitiveKey];
        var originalFails = state.ShrinkRun(key, value);
        if (!originalFails)
            return ShrinkOutcome.Irrelevant;
        var filtered = primitiveVals.Where(val => shrinkingGuard(val)).ToArray();
        bool failureAlwaysOccurs =
            filtered
                .Where(x => !Equals(x, value))
                .All(x => state.ShrinkRun(key, x));
        return failureAlwaysOccurs ? ShrinkOutcome.Irrelevant : ShrinkOutcome.Report(QuickAcidStringify.Default()(value));
    }
}