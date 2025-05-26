
using System.Collections;
using QuickAcid.Bolts.TheyCanFade;

namespace QuickAcid.Bolts.ShrinkStrats.Collections;

public class ShrinkEachElementStrategy : ICollectionShrinkStrategy
{
    public IEnumerable<ShrinkTrace> Shrink<T>(QAcidState state, string key, T value)
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

            using (state.Memory.ScopedSwap(key, theList))
            {
                var swapper = new MemoryLens(
                    list => ((IList)list)[ix]!,
                    (list, element) =>
                    {
                        if (element == null || elementType.IsAssignableFrom(element.GetType()))
                            ((IList)list)[ix] = element;
                        return list;
                    });
                using (state.Memory.NestedValue(swapper))
                {
                    var shrinkOutcome = ShrinkStrategyPicker.Input(state, key, before);
                    if (shrinkOutcome is ShrinkOutcome.ReportedOutcome(var msg))
                    {
                        //shrinkValues.Add($"{msg}");
                        yield return new ShrinkTrace
                        {
                            Key = $"{key}.{index}",
                            Original = before,
                            Result = before,
                            Intent = ShrinkIntent.Keep,
                            Strategy = "ShrinkEachElementStrategy",
                            Message = "Minimal value causing failure"
                        };
                    }
                    else
                    {
                        //shrinkValues.Add($"_");
                        yield return new ShrinkTrace
                        {
                            Key = $"{key}.{index}",
                            Original = before,
                            Result = null,
                            Intent = ShrinkIntent.Irrelevant,
                            Strategy = "ShrinkEachElementStrategy",
                            Message = "Input value is irrelevant to failure"
                        };
                    }
                    theList[ix] = before;
                    index++;
                }
            }
            // state.Memory.GetNestedValue = list => ((IList)list)[ix]!;
            // state.Memory.SetNestedValue = element =>
            // {
            //     if (element == null || elementType.IsAssignableFrom(element.GetType()))
            //     {
            //         theList[ix] = element;
            //     }
            //     else
            //     {
            //         throw new Exception("Ouch, QuickAcid Went BOOM !");
            //     }
            //     return theList;
            // };
            //     var shrinkOutcome = ShrinkStrategyPicker.Input(state, key, before);
            // if (shrinkOutcome is ShrinkOutcome.ReportedOutcome(var msg))
            // {
            //     shrinkValues.Add($"{msg}");
            // }
            // else
            //     shrinkValues.Add($"_");
            // theList[ix] = before;
            // index++;
            // state.Memory.GetNestedValue = null;
            // state.Memory.SetNestedValue = null;
        }
        //return (T)theList;
    }
}
