using System.Reflection;
using QuickMGenerate.UnderTheHood;

namespace QuickAcid.Bolts;

public abstract record ShrinkOutcome
{
    public static readonly ShrinkOutcome Irrelevant = new IrrelevantOutcome();
    public static ShrinkOutcome Report(string message) => new ReportedOutcome(message);

    public sealed record IrrelevantOutcome : ShrinkOutcome;
    public sealed record ReportedOutcome(string Message) : ShrinkOutcome;

    public override string ToString() => this switch
    {
        ReportedOutcome r => r.Message,
        IrrelevantOutcome => "Irrelevant",
        _ => "Unknown"
    };
}

public class Shrink
{
    private static readonly Dictionary<Type, object[]> PrimitiveValues =
        new Dictionary<Type, object[]>
        {
            {typeof(int), new object[] { -1000, -1, 0, 1, 1000 }},
            {typeof(string), new object[] { null, "", new string('-', 256), new string('-', 1024) }},
        };

    public static ShrinkOutcome Input<T>(QAcidState state, string key, T value, Func<object, bool> shrinkingGuard)
    {
        var shrunk = ShrinkOutcome.Irrelevant;
        if (typeof(IEnumerable<int>).IsAssignableFrom(typeof(T)))
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

    public static IEnumerable<IEnumerable<T>> GetPowerSet<T>(List<T> list)
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
        var theList = ((IEnumerable<int>)value).ToList();
        int index = 0;
        while (index < theList.Count)
        {
            var ix = index;
            var before = theList[ix];
            var primitiveVals = new[] { -1, 0, 1 };
            var removed = false;
            foreach (var primitiveVal in primitiveVals.Where(p => !p.Equals(before)))
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
        return ShrinkOutcome.Report($"[ {string.Join(", ", theList.Select(v => v.ToString()))} ]");
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
