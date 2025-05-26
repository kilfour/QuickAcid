namespace QuickAcid.Bolts.ShrinkStrats.Collections;

public class RemoveOneByOneStrategy : ICollectionShrinkStrategy
{
    public IEnumerable<ShrinkTrace> Shrink<T>(QAcidState state, string key, T value)
    {
        var theList = CloneList.AsOriginalType(value!);
        int index = 0;
        int indexKey = -1;
        //var existingTraces = state.Memory.ForThisExecution().GetDecorated(key).ShrinkTraces;
        while (index < theList.Count)
        {
            indexKey++;
            // if (existingTraces.Any(a => a.Key == $"{key}.{index}" && a.IsRemoved))
            // {
            //     index++;
            //     continue;
            // }
            var ix = index;
            var before = theList[ix];
            theList.RemoveAt(ix);
            if (state.ShrinkRunReturnTrueIfFailed(key, theList!))
            {
                yield return new ShrinkTrace
                {
                    Key = $"{key}.{indexKey}",
                    Original = before,
                    Result = null,
                    Intent = ShrinkIntent.Remove,
                    Strategy = "RemoveOneByOneStrategy",
                    Message = "Input value is irrelevant to failure"
                };
                continue;
            }
            yield return new ShrinkTrace
            {
                Key = $"{key}.{indexKey}",
                Original = before,
                Result = before,
                Intent = ShrinkIntent.Keep,
                Strategy = "RemoveOneByOneStrategy",
                Message = "Minimal value causing failure"
            };
            //shrinkValues.Add(QuickAcidStringify.Default()(before!));
            theList.Insert(ix, before);
            index++;
        }
    }
}
