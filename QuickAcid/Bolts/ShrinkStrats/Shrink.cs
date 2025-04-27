using System.Collections;

namespace QuickAcid.Bolts.ShrinkStrats;

public class Shrink
{
    public static ShrinkOutcome Input<T>(QAcidState state, string key, T value, Func<object, bool> shrinkingGuard)
    {
        var shrunk = ShrinkOutcome.Irrelevant;

        var actualType = typeof(T) == typeof(object) && value != null
            ? value.GetType()
            : typeof(T);

        if (typeof(IEnumerable).IsAssignableFrom(actualType))
        {
            return new EnumerableShrinkStrategy().Shrink(state, key, value, _ => true);
        }
        var primitiveKey = PrimitiveShrinkStrategy.PrimitiveValues.Keys.FirstOrDefault(k => k.IsAssignableFrom(actualType));
        if (primitiveKey != null)
        {
            return new PrimitiveShrinkStrategy().Shrink(state, key, value, shrinkingGuard);
        }

        if (actualType.IsClass)
        {
            return new ObjectShrinkStrategy().Shrink(state, key, value, shrinkingGuard);
        }

        return shrunk;
    }
}
