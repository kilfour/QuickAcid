using System.Collections;

namespace QuickAcid.Bolts.ShrinkStrats;

public class ElementwiseShrinkStrategy
{
    public ShrinkOutcome Shrink<T>(QAcidState state, string key, T value)
    {
        var theList = CloneAsOriginalTypeList(value!);
        var shrinkValues = new List<string>();

        for (int ix = 0; ix < theList.Count; ix++)
        {
            var original = theList[ix];
            var valueType = original?.GetType() ?? typeof(object);
            var elementType = valueType.IsGenericType
                ? valueType.GetGenericArguments().First()
                : valueType;

            state.Memory.GetNestedValue = list => ((IList)list)[ix]!;
            state.Memory.SetNestedValue = element =>
            {
                if (element == null || elementType.IsAssignableFrom(element.GetType()))
                {
                    theList[ix] = element;
                }
                else
                {
                    throw new Exception("QuickAcid Elementwise shrink blew up");
                }
                return theList;
            };

            var shrinkOutcome = ShrinkStrats.Shrink.Input(state, key, original);

            if (shrinkOutcome is ShrinkOutcome.ReportedOutcome(var msg))
            {
                shrinkValues.Add($"[{ix}]={msg}");
            }

            if (shrinkOutcome is ShrinkOutcome.IrrelevantOutcome)
            {
                shrinkValues.Add($"[{ix}]= _");
            }

            theList[ix] = original; // reset to original
            state.Memory.GetNestedValue = null;
            state.Memory.SetNestedValue = null;
        }

        return ShrinkOutcome.Report(string.Join(", ", shrinkValues));
    }

    public static IList CloneAsOriginalTypeList(object value)
    {
        if (value is not IEnumerable enumerable)
            throw new ArgumentException("Value is not an IEnumerable", nameof(value));

        var valueType = value.GetType();
        Type elementType;

        if (valueType.IsArray)
        {
            elementType = valueType.GetElementType()!;
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
