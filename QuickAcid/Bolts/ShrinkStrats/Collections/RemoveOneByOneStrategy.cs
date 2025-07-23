using QuickAcid.Bolts.TheyCanFade;

namespace QuickAcid.Bolts.ShrinkStrats.Collections;

public class RemoveOneByOneStrategy : ICollectionShrinkStrategy
{
    public void Shrink<T>(QAcidState state, string key, T value, string fullKey)
    {
        var theList = CloneList.AsOriginalType(value!);
        int index = 0;
        int indexKey = -1;
        while (index < theList.Count)
        {
            indexKey++;
            var ix = index;
            var before = theList[ix];
            theList.RemoveAt(ix);
            if (state.VerifyIf.RunFailed(key, theList!))
            {
                state.GetExecutionContext().Trace(key, ShrinkKind.KeepSameKind, new ShrinkTrace
                {
                    ExecutionId = -1,
                    Key = $"{fullKey}.{indexKey}",
                    Name = indexKey.ToString(),
                    Original = before,
                    Result = null,
                    Intent = ShrinkIntent.Remove,
                    Strategy = "RemoveOneByOneStrategy"
                });
                continue;
            }
            state.GetExecutionContext().Trace(key, ShrinkKind.KeepSameKind, new ShrinkTrace
            {
                ExecutionId = -1,
                Key = $"{fullKey}.{indexKey}",
                Name = indexKey.ToString(),
                Original = before,
                Result = before,
                Intent = ShrinkIntent.Keep,
                Strategy = "RemoveOneByOneStrategy"
            });
            theList.Insert(ix, before);
            index++;
        }
    }
}
