
using System.Collections;

namespace QuickAcid.Bolts.ShrinkStrats;

public class EnumerableShrinkStrategy
{
    public ShrinkOutcome Shrink<T>(QAcidState state, string key, T value)
    {
        var theList = CloneAsOriginalTypeList(value);
        int index = 0;
        var shrinkValues = new List<string>();
        while (index < theList.Count)
        {
            var ix = index;
            var before = theList[ix];
            //-------------------------------------------------------
            // First try removing the element  
            //-------------------------------------------------------
            theList.RemoveAt(ix);
            if (state.ShrinkRunReturnTrueIfFailed(key, theList!))
            {
                continue;
            }
            theList.Insert(ix, before);
            //-------------------------------------------------------
            // Then try if element value is important 
            //-------------------------------------------------------
            var valueType = before.GetType();
            var elementType = valueType.IsGenericType
                ? valueType.GetGenericArguments().First()
                : typeof(object);

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
                    throw new Exception("Ouch, QuickAcid Went BOOM !");
                }
                return theList;
            };
            var shrinkOutcome = ShrinkStrats.Shrink.Input(state, key, before);

            if (shrinkOutcome == ShrinkOutcome.Irrelevant)
            {
                // theList.RemoveAt(index);
                shrinkValues.Add($"_");
                removed = true;
            }
            else if (shrinkOutcome is ShrinkOutcome.ReportedOutcome(var msg))
            {
                shrinkValues.Add($"{msg}");
            }
            if (!removed)
            {
                theList[ix] = before;
            }
            index++;
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
