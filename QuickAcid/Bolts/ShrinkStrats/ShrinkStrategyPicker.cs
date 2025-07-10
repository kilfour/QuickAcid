using System.Collections;
using QuickAcid.Bolts.ShrinkStrats.Collections;

namespace QuickAcid.Bolts.ShrinkStrats;

public class ShrinkStrategyPicker
{
    public static void Input<T>(QAcidState state, string key, T value, string fullKey)
    {
        var actualType = typeof(T) == typeof(object) && value != null
            ? value.GetType()
            : typeof(T);

        var normalizedType = Nullable.GetUnderlyingType(actualType) ?? actualType;
        var customShrinker = state.TryGetShrinker<T>();
        if (customShrinker != null)
        {
            new CustomShrinkStrategy<T>(customShrinker).Shrink(state, key, value);
            return;
        }

        var primitiveKey = PrimitiveShrinkStrategy.PrimitiveValues.Keys.FirstOrDefault(k => k.IsAssignableFrom(normalizedType));
        if (primitiveKey != null)
        {
            new PrimitiveShrinkStrategy().Shrink(state, key, value, fullKey);
            return;
        }

        if (typeof(IEnumerable).IsAssignableFrom(actualType))
        {
            new EnumerableShrinkStrategy().Shrink(state, key, value, fullKey);
            return;
        }

        if (actualType.IsClass)
        {
            new ObjectShrinkStrategy().Shrink(state, key, value, fullKey);
            return;
        }
    }
}
