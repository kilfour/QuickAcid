using System.Collections;
using System.Reflection;
using QuickMGenerate.UnderTheHood;

namespace QuickAcid.Bolts;

public class Shrink
{
    private static readonly Dictionary<Type, object[]> PrimitiveValues =
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

    public static ShrinkOutcome Input<T>(QAcidState state, string key, T value, Func<object, bool> shrinkingGuard)
    {
        var shrunk = ShrinkOutcome.Irrelevant;
        if (typeof(IEnumerable).IsAssignableFrom(typeof(T)))
        {
            return ShrinkIEnumerable(state, key, value);
        }
        var primitiveKey = PrimitiveValues.Keys.FirstOrDefault(k => k.IsAssignableFrom(typeof(T)));
        if (primitiveKey != null)
        {
            return ShrinkPrimitive(state, key, value, PrimitiveValues[primitiveKey], shrinkingGuard);
        }

        if (typeof(T).IsClass)
        {
            shrunk = HandleProperties(state, key, value);

            if (shrunk == ShrinkOutcome.Irrelevant)
            {
                var oldValues = new Dictionary<string, object>();
                foreach (var propertyInfo in value.GetType().GetProperties(MyBinding.Flags))
                {
                    oldValues[propertyInfo.Name] = propertyInfo.GetValue(value);
                }

                foreach (var set in GetPowerSet(value.GetType().GetProperties(MyBinding.Flags).ToList()))
                {
                    if (shrunk != ShrinkOutcome.Irrelevant)
                        break;

                    foreach (var propertyInfo in set)
                    {
                        SetPropertyValue(propertyInfo, value, null);
                    }

                    if (!state.ShrinkRun(key, value))
                    {
                        shrunk = ShrinkOutcome.Report(string.Join(", ", set.Select(x => $"{x.Name} : {oldValues[x.Name]}")));
                    }

                    foreach (var propertyInfo in set)
                    {
                        SetPropertyValue(propertyInfo, value, oldValues[propertyInfo.Name]);
                    }
                }
            }
        }
        return shrunk;
    }

    private static IEnumerable<IEnumerable<T>> GetPowerSet<T>(List<T> list)
    {
        return from m in Enumerable.Range(0, 1 << list.Count)
               select
                   from i in Enumerable.Range(0, list.Count)
                   where (m & 1 << i) != 0
                   select list[i];
    }

    private static ShrinkOutcome HandleProperties<T>(QAcidState state, object key, T value)
    {
        var messages = value.GetType()
            .GetProperties(MyBinding.Flags)
            .Select(p => HandleProperty(state, key, value, p))
            .OfType<ShrinkOutcome.ReportedOutcome>()
            .Select(r => r.Message)
            .ToList();

        return messages.Any()
            ? ShrinkOutcome.Report(string.Join(", ", messages))
            : ShrinkOutcome.Irrelevant;
    }

    private static ShrinkOutcome HandleProperty(QAcidState state, object key, object value, PropertyInfo propertyInfo)
    {
        var propertyValue = propertyInfo.GetValue(value);
        var primitiveKey = PrimitiveValues.Keys.FirstOrDefault(k => k.IsAssignableFrom(propertyInfo.PropertyType));
        if (primitiveKey != null)
        {
            foreach (var primitiveValue in PrimitiveValues[primitiveKey])
            {
                SetPropertyValue(propertyInfo, value, primitiveValue);
                if (!state.ShrinkRun(key, value))
                {
                    SetPropertyValue(propertyInfo, value, propertyValue);
                    return ShrinkOutcome.Report($"{propertyInfo.Name} : {propertyValue}");
                }
            }
            SetPropertyValue(propertyInfo, value, propertyValue);
        }
        return ShrinkOutcome.Irrelevant;
    }

    private static void SetPropertyValue(PropertyInfo propertyInfo, object target, object value)
    {
        var prop = propertyInfo;
        if (!prop.CanWrite)
            prop = propertyInfo.DeclaringType.GetProperty(propertyInfo.Name);

        if (prop.CanWrite) // todo check this
            prop.SetValue(target, value, null);
    }

    private static ShrinkOutcome ShrinkIEnumerable<T>(QAcidState state, object key, T value)
    {
        var theList = CloneAsOriginalTypeList(value);
        int index = 0;
        while (index < theList.Count)
        {
            var ix = index;
            var before = theList[ix];
            var primitiveKey = before.GetType();
            var primitiveVals = PrimitiveValues[primitiveKey];
            var removed = false;
            foreach (var primitiveVal in primitiveVals.Where(p => p == null || !p.Equals(before)))
            {
                theList[ix] = primitiveVal;
                var shrinkstate = state.ShrinkRun(key, theList);
                if (shrinkstate)
                {
                    theList.RemoveAt(index);
                    removed = true;
                    break;
                }
            }
            if (!removed)
            {
                theList[ix] = before;
                index++;
            }
        }
        return ShrinkOutcome.Report($"[ {string.Join(", ", theList.Cast<object>().Select(v => v.ToString()))} ]");
    }

    public static IList CloneAsOriginalTypeList(object value)
    {
        if (value is not IEnumerable enumerable)
            throw new ArgumentException("Value is not an IEnumerable", nameof(value));

        var valueType = value.GetType();
        var elementType = valueType.IsGenericType
            ? valueType.GetGenericArguments().FirstOrDefault()
            : typeof(object); // fallback, though it shouldn't happen with real List<T>

        if (elementType == null)
            throw new InvalidOperationException("Could not determine the element type of the list.");

        var typedListType = typeof(List<>).MakeGenericType(elementType);
        var typedList = (IList)Activator.CreateInstance(typedListType)!;

        foreach (var item in enumerable)
        {
            typedList.Add(item);
        }

        return typedList;
    }
    private static ShrinkOutcome ShrinkPrimitive(QAcidState state, string key, object value, object[] primitiveVals, Func<object, bool> shrinkingGuard)
    {
        var originalFails = state.ShrinkRun(key, value);
        if (!originalFails)
            return ShrinkOutcome.Irrelevant;
        var filtered = primitiveVals.Where(val => shrinkingGuard(val)).ToArray();
        bool failureAlwaysOccurs =
            filtered
                .Where(x => !Equals(x, value))
                .All(x => state.ShrinkRun(key, x));
        return failureAlwaysOccurs ? ShrinkOutcome.Irrelevant : ShrinkOutcome.Report(value.ToString());
    }
}
