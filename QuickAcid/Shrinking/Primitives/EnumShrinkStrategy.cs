using System.Reflection;
using QuickAcid.Bolts;
using QuickAcid.TheyCanFade;

namespace QuickAcid.Shrinking.Primitives;

public class EnumShrinkStrategy
{
    public void Shrink<T>(QAcidState state, string key, T value, string fullKey)
    {
        var actualType = typeof(T) == typeof(object) && value != null
            ? value.GetType()
            : typeof(T);

        var enumVals = GetEnumValues(actualType);
        var originalFails = state.VerifyIf.RunFailed(key, value!);
        if (!originalFails)
            return;

        foreach (var candidate in enumVals.Where(x => !Equals(x, value)))
        {
            if (state.VerifyIf.RunPassed(key, candidate))
            {
                state.CurrentExecutionContext().Trace(key, ShrinkKind.PrimitiveKind, new ShrinkTrace
                {
                    ExecutionId = -1,
                    Key = fullKey,
                    Name = fullKey.Split(".").Last(),
                    Original = value,
                    Result = candidate,
                    Intent = ShrinkIntent.Keep,
                    Strategy = "EnumShrink"
                });
                return;
            }

            state.CurrentExecutionContext().Trace(key, ShrinkKind.PrimitiveKind, new ShrinkTrace
            {
                ExecutionId = -1,
                Key = fullKey,
                Name = fullKey.Split(".").Last(),
                Original = value,
                Result = null,
                Intent = ShrinkIntent.Irrelevant,
                Strategy = "EnumShrink"
            });
        }
    }

    private static IEnumerable<object> GetEnumValues(Type type)
    {
        if (!type.IsEnum) return Array.Empty<object>();
        return Enum.GetValues(type).Cast<object>();
    }
}
