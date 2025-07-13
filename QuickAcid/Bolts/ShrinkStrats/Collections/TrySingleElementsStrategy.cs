using QuickAcid.Bolts.TheyCanFade;

namespace QuickAcid.Bolts.ShrinkStrats.Collections;

public class TrySingleElementsStrategy : ICollectionShrinkStrategy
{
    public void Shrink<T>(QAcidState state, string key, T value, string fullKey)
    {
        var original = CloneList.AsOriginalType(value!);
        var empty = CloneList.AsOriginalType(value!);

        var listCount = original.Count;

        for (int i = 0; i < listCount; i++)
            empty[i] = GetPlaceholder(original[i]!);

        for (int i = 0; i < listCount; i++)
        {
            var cloned = CloneList.AsOriginalType(empty);
            cloned[i] = original[i];
            bool shrinkAccepted = state.ShrinkRunReturnTrueIfFailed(key, cloned);
            if (shrinkAccepted)
            {
                for (int j = 0; j < listCount; j++)
                {
                    state.Trace(key, ShrinkKind.PrimitiveKind, new ShrinkTrace
                    {
                        Key = $"{fullKey}.[{j}]",
                        Name = $"[{j}]",
                        Original = original[j],
                        Result = original[j],
                        Intent = i == j ? ShrinkIntent.Keep : ShrinkIntent.Irrelevant,
                        Strategy = "PrimitiveShrink"
                    });
                }
                break;
            }
        }
    }

    private static object GetPlaceholder(object original)
    {
        var type = original.GetType();
        return type.IsValueType ? Activator.CreateInstance(type)! : null!;
    }
}

