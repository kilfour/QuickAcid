using System.Collections;
using QuickAcid.Bolts.ShrinkStrats.Collections;

namespace QuickAcid.Bolts.ShrinkStrats;

public class ShrinkStrategyPicker
{
    public static ShrinkOutcome Input<T>(QAcidState state, string key, T value)
    {
        var actualType = typeof(T) == typeof(object) && value != null
            ? value.GetType()
            : typeof(T);

        var normalizedType = Nullable.GetUnderlyingType(actualType) ?? actualType;
        var customShrinker = state.TryGetShrinker<T>();
        if (customShrinker != null)
            return new CustomShrinkStrategy<T>(customShrinker).Shrink(state, key, value);

        var primitiveKey = PrimitiveShrinkStrategy.PrimitiveValues.Keys.FirstOrDefault(k => k.IsAssignableFrom(normalizedType));
        if (primitiveKey != null)
        {
            var trace = new PrimitiveShrinkStrategy().Shrink(state, key, value);
            if (trace.Single().IsIrrelevant)
                return ShrinkOutcome.Irrelevant;
            else
                return ShrinkOutcome.Report(QuickAcidStringify.Default()(value!));
        }

        if (typeof(IEnumerable).IsAssignableFrom(actualType))
            return new EnumerableShrinkStrategy().Shrink(state, key, value);


        if (actualType.IsClass)
            return new ObjectShrinkStrategy().Shrink(state, key, value);

        return ShrinkOutcome.Report(QuickAcidStringify.Default()($"No Shrinker for: {value}"));
    }
}
