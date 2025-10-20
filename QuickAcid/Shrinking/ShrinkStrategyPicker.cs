using System.Collections;
using QuickAcid.Bolts;
using QuickAcid.Shrinking.Collections;
using QuickAcid.Shrinking.Custom;
using QuickAcid.Shrinking.Objects;
using QuickAcid.Shrinking.Primitives;
using QuickAcid.TheyCanFade;

namespace QuickAcid.Shrinking;

public class ShrinkStrategyPicker
{
    public static void Input<T>(QAcidState state, string key, T value, string fullKey)
    {
        var actualType = typeof(T) == typeof(object) && value != null
            ? value.GetType()
            : typeof(T);

        var normalizedType = Nullable.GetUnderlyingType(actualType) ?? actualType;

        var customReducer = state.ReducersRegistry.TryGet<T>();
        if (customReducer != null)
        {
            new CustomReductionStrategy<T>(customReducer).Reduce(state, key, value!);
            return;
        }

        var customShrinker = state.ShrinkingRegistry.TryGetShrinker<T>();
        if (customShrinker != null)
        {
            new CustomShrinkStrategy(new ShrinkerBox<T>(customShrinker)).Shrink(state, key, value!);
            return;
        }

        var primitiveKey = PrimitiveShrinkStrategy.PrimitiveValues.Keys.FirstOrDefault(k => k.IsAssignableFrom(normalizedType));
        if (primitiveKey != null)
        {
            new PrimitiveShrinkStrategy().Shrink(state, key, value, fullKey);
            return;
        }

        if (actualType.IsEnum)
        {
            new EnumShrinkStrategy().Shrink(state, key, value, fullKey);
            return;
        }

        if (typeof(IEnumerable).IsAssignableFrom(actualType))
        {
            new EnumerableShrinkStrategy().Shrink(state, key, value, fullKey);
            return;
        }

        if (actualType.IsClass)
        {
            CompositeObjectShrinkStrategy.Shrink(state, key, value, fullKey);
            return;
        }

        state.CurrentExecutionContext().ShrinkTrace(key, ShrinkKind.Unknown, new ShrinkTrace
        {
            ExecutionId = -1,
            Key = fullKey,
            Name = fullKey.Split(".").Last(),
            Original = value,
            Result = null,
            Intent = ShrinkIntent.Keep,
            Strategy = "NoneFound"
        });
    }
}
