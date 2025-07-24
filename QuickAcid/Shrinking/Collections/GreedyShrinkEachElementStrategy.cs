using QuickAcid.Bolts;

namespace QuickAcid.Shrinking.Collections;

public class GreedyShrinkEachElementStrategy : ICollectionShrinkStrategy
{
    public void Shrink<T>(QAcidState state, string key, T value, string fullKey)
    {
        var theList = CloneList.AsOriginalType(value!);
        int index = 0;
        var indexKey = 0;
        var mapper = new Dictionary<int, int>();
        while (index < theList.Count)
        {
            mapper[index] = indexKey;
            var removeForShrinking =
                state.Memory.ForThisExecution().GetDecorated(key).GetShrinkTraces()
                    .Any(a => a.Key == $"{fullKey}.{indexKey}" && a.IsRemoved);
            if (removeForShrinking)
                theList.RemoveAt(index);
            else
                index++;
            indexKey++;
        }
        new ShrinkEachElementStrategy { Mapper = mapper }.Shrink(state, key, theList, fullKey);
    }

    private static object GetPlaceholder(object original)
    {
        var type = original.GetType();
        return type.IsValueType ? Activator.CreateInstance(type)! : null!;
    }
}
