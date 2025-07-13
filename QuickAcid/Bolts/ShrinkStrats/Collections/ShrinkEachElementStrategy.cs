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
            var removeForShrinking =
                state.Memory.ForThisExecution().GetDecorated(key).GetShrinkTraces()
                    .Any(a => a.Key == $"{fullKey}.{index}" && a.IsRemoved);
            if (removeForShrinking)
                theList.RemoveAt(index);
            else
                index++;
        }

        index = 0;
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
                    var removeForShrinking =
                        state.Memory.ForThisExecution().GetDecorated(key).GetShrinkTraces()
                            .Any(a => a.Key == $"{fullKey}.{index}" && a.IsIrrelevant);
                    if (removeForShrinking)
                        theList[ix] = GetPlaceholder(theList[ix]!);
                    else
                        theList[ix] = before;
                    index++;
                }
            }


            // if (removeForShrinking != null)
            // {
            //     theList[ix] = GetPlaceholder(removeForShrinking.Original!);
            // }
        }
    }

    private static object GetPlaceholder(object original)
    {
        var type = original.GetType();
        return type.IsValueType ? Activator.CreateInstance(type)! : null!;
    }
}
