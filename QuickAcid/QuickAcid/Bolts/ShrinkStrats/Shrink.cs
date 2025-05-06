using System.Collections;

namespace QuickAcid.Bolts.ShrinkStrats;

public class Shrink
{
    public static ShrinkOutcome Input<T>(QAcidState state, string key, T value, Func<object, bool> shrinkingGuard)
    {
        var actualType = typeof(T) == typeof(object) && value != null
            ? value.GetType()
            : typeof(T);

        var normalizedType = Nullable.GetUnderlyingType(actualType) ?? actualType;
        var customShrinker = state.TryGetShrinker<T>();
        if (customShrinker != null)
        {
            return new CustomShrinkStrategy<T>(customShrinker).Shrink(state, key, value, shrinkingGuard);
        }

        var primitiveKey = PrimitiveShrinkStrategy.PrimitiveValues.Keys.FirstOrDefault(k => k.IsAssignableFrom(normalizedType));
        if (primitiveKey != null)
        {
            return new PrimitiveShrinkStrategy().Shrink(state, key, value, shrinkingGuard);
        }

        if (typeof(IEnumerable).IsAssignableFrom(actualType))
        {
            return new EnumerableShrinkStrategy().Shrink(state, key, value, _ => true);
        }

        if (actualType.IsClass)
        {
            return new ObjectShrinkStrategy().Shrink(state, key, value, shrinkingGuard);
        }

        return ShrinkOutcome.Report(QuickAcidStringify.Default()($"No Shrinker for: {value}"));
    }
}
