
using System.Collections;

namespace QuickAcid.Bolts;

public class EnumerableShrinkStrategy : IShrinkStrategy
{
    public ShrinkOutcome Shrink<T>(QAcidState state, string key, T value, Func<object, bool> shrinkingGuard)
    {
        var theList = CloneAsOriginalTypeList(value);
        int index = 0;
        while (index < theList.Count)
        {
            var ix = index;
            var before = theList[ix];
            var primitiveKey = before.GetType();
            var primitiveVals = PrimitiveShrinkStrategy.PrimitiveValues[primitiveKey];
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

    private static IList CloneAsOriginalTypeList(object value)
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
}
