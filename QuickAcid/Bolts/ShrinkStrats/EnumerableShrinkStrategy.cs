
using System.Collections;

namespace QuickAcid.Bolts.ShrinkStrats;

public class EnumerableShrinkStrategy //: IShrinkStrategy
{
    public ShrinkOutcome Shrink<T>(QState state, string key, T value, Func<object, bool> shrinkingGuard)
    {
        var theList = CloneAsOriginalTypeList(value);
        int index = 0;
        var shrinkValues = new List<string>();
        while (index < theList.Count)
        {
            var ix = index;
            var before = theList[ix];
            var valueType = before.GetType();
            var elementType = valueType.IsGenericType
                ? valueType.GetGenericArguments().First()
                : typeof(object);
            //-------------------------------------------------------
            var removed = false;
            state.Memory.GetNestedValue = list => ((IList)list)[ix];
            state.Memory.SetNestedValue = element =>
            {
                if (element == null || elementType.IsAssignableFrom(element.GetType()))
                {
                    theList[ix] = element;
                }
                else
                {
                    throw new Exception("Ouch, QUuickAcid Went BOOM !");
                }
                return theList;
            };
            var shrinkOutcome = ShrinkStrats.Shrink.Input(state, key, before, _ => true);

            if (shrinkOutcome == ShrinkOutcome.Irrelevant)
            {
                theList.RemoveAt(index);
                removed = true;
            }
            else if (shrinkOutcome is ShrinkOutcome.ReportedOutcome(var msg))
            {
                shrinkValues.Add($"{msg}");
            }
            if (!removed)
            {
                theList[ix] = before;
                index++;
            }
            state.Memory.GetNestedValue = null;
            state.Memory.SetNestedValue = null;
        }
        return ShrinkOutcome.Report($"[ {string.Join(", ", shrinkValues)} ]");
    }

    public static IList CloneAsOriginalTypeList(object value)
    {
        if (value is not IEnumerable enumerable)
            throw new ArgumentException("Value is not an IEnumerable", nameof(value));

        var valueType = value.GetType();
        Type elementType;

        if (valueType.IsArray)
        {
            elementType = valueType.GetElementType(); // ✅ Correct for arrays
        }
        else if (valueType.IsGenericType)
        {
            elementType = valueType.GetGenericArguments().First();
        }
        else
        {
            elementType = typeof(object);
        }

        var typedListType = typeof(List<>).MakeGenericType(elementType);
        var typedList = (IList)Activator.CreateInstance(typedListType)!;

        foreach (var item in enumerable)
        {
            typedList.Add(item);
        }

        return typedList;
    }
}
