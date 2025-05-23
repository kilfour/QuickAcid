namespace QuickAcid.Bolts.ShrinkStrats.Collections;

public class RemoveOneByOneStrategy : ICollectionShrinkStrategy
{
    public T Shrink<T>(QAcidState state, string key, T value, List<string> shrinkValues)
    {
        var theList = CloneList.AsOriginalType(value!);
        int index = 0;
        while (index < theList.Count)
        {
            var ix = index;
            var before = theList[ix];
            theList.RemoveAt(ix);
            if (state.ShrinkRunReturnTrueIfFailed(key, theList!))
            {
                shrinkValues.Add("_");
                continue;
            }
            shrinkValues.Add(QuickAcidStringify.Default()(before!));
            theList.Insert(ix, before);
            index++;
        }
        return (T)theList;
    }
}
