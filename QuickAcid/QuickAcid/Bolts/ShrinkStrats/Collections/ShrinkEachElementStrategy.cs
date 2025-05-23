
using System.Collections;

namespace QuickAcid.Bolts.ShrinkStrats.Collections;

public class ShrinkEachElementStrategy : ICollectionShrinkStrategy
{
    public T Shrink<T>(QAcidState state, string key, T value, List<string> shrinkValues)
    {
        var theList = CloneList.AsOriginalType(value!);
        int index = 0;
        while (index < theList.Count)
        {
            var ix = index;
            var before = theList[ix];
            var valueType = before!.GetType();
            var elementType = valueType.IsGenericType
                ? valueType.GetGenericArguments().First()
                : typeof(object);

            state.Memory.GetNestedValue = list => ((IList)list)[ix]!;
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
            var shrinkOutcome = ShrinkStrategyPicker.Input(state, key, before);
            if (shrinkOutcome is ShrinkOutcome.ReportedOutcome(var msg))
            {
                shrinkValues.Add($"{msg}");
            }
            else
                shrinkValues.Add($"_");
            theList[ix] = before;
            index++;
            state.Memory.GetNestedValue = null;
            state.Memory.SetNestedValue = null;
        }
        return (T)theList;
    }
}
