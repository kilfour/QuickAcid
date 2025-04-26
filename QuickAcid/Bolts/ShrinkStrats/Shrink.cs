using System.Collections;

namespace QuickAcid.Bolts.ShrinkStrats;

public class Shrink
{
    public static ShrinkOutcome Input<T>(QAcidState state, string key, T value, Func<object, bool> shrinkingGuard)
    {
        var shrunk = ShrinkOutcome.Irrelevant;
        if (typeof(IEnumerable).IsAssignableFrom(typeof(T)))
        {
            return new EnumerableShrinkStrategy().Shrink(state, key, value, _ => true);
        }
        var primitiveKey = PrimitiveShrinkStrategy.PrimitiveValues.Keys.FirstOrDefault(k => k.IsAssignableFrom(typeof(T)));
        if (primitiveKey != null)
        {
            return new PrimitiveShrinkStrategy().Shrink(state, key, value, shrinkingGuard);
        }
        if (typeof(T).IsClass)
        {
            return new ObjectShrinkStrategy().Shrink(state, key, value, shrinkingGuard);
        }
        return shrunk;
    }
}
