using System.Collections;
using QuickAcid.Bolts.TheyCanFade;

namespace QuickAcid.Bolts.ShrinkStrats.Collections;

public class ShrinkEachElementStrategy : ICollectionShrinkStrategy
{
    public void Shrink<T>(QAcidState state, string key, T value, string fullKey)
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
                    ShrinkStrategyPicker.Input(state, key, before, $"{fullKey}.{index}");

                    //var traces = state.Memory.TracesForThisExecution().Where(a => a.Key.StartsWith($"{fullKey}.{index}"));

                    // if (shrinkOutcome is ShrinkOutcome.ReportedOutcome(var msg))
                    // {
                    //     state.Trace(key, new ShrinkTrace
                    //     {
                    //         Key = $"{fullKey}.{index}",
                    //         Original = before,
                    //         Result = before,
                    //         Intent = ShrinkIntent.Keep,
                    //         Strategy = "ShrinkEachElementStrategy",
                    //         Message = "Minimal value causing failure"
                    //     });
                    // }
                    // else
                    // {
                    //     state.Trace(key, new ShrinkTrace
                    //     {
                    //         Key = $"{fullKey}.{index}",
                    //         Original = before,
                    //         Result = null,
                    //         Intent = ShrinkIntent.Irrelevant,
                    //         Strategy = "ShrinkEachElementStrategy",
                    //         Message = "Input value is irrelevant to failure"
                    //     });
                    // }
                    theList[ix] = before;
                    index++;
                }
            }
        }
    }
}
