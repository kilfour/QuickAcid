
using System.Reflection;
using QuickAcid.Bolts.TheyCanFade;
using QuickFuzzr;
using QuickFuzzr.UnderTheHood;

namespace QuickAcid.Bolts.ShrinkStrats.Objects;

public class ObjectShrinkStrategy : IObjectShrinkStrategy
{
    public void Shrink<T>(QAcidState state, string key, T value, string fullKey)
    {
        state.Trace(key, ShrinkKind.ObjectKind, new ShrinkTrace
        {
            ExecutionId = -1,
            Key = fullKey,
            Name = fullKey.Split(".").Last(),
            Original = value,
            Result = value,
            Intent = ShrinkIntent.Keep,
            Strategy = "ObjectShrinkStrategy"
        });
        HandleProperties(state, key, value, fullKey);
    }

    private static void HandleProperties<T>(QAcidState state, string key, T value, string fullKey)
    {
        value!.GetType()
            .GetProperties(MyBinding.Flags)
            .Where(p =>
                p.CanRead &&
                p.GetIndexParameters().Length == 0 &&
                p.SetMethod is not null &&
                p.SetMethod.IsPublic)
            .ForEach(p => HandleProperty(state, key, value, p, fullKey));
    }

    private static void HandleProperty<T>(QAcidState state, string key, T value, PropertyInfo propertyInfo, string fullKey)
    {
        var propertyValue = propertyInfo.GetValue(value);
        var swapper = new MemoryLens(
            obj => propertyInfo.GetValue(obj)!,
            (val, propValue) => { SetPropertyValue(propertyInfo, value!, propValue); return val; });
        using (state.Memory.NestedValue(swapper))
        {
            var customShrinker = state.ShrinkingRegistry.TryGetPropertyShrinker<T>(propertyInfo);
            if (customShrinker != null)
                new CustomShrinkStrategy(customShrinker).Shrink(state, key, propertyValue!);
            else
                ShrinkStrategyPicker.Input(state, key, propertyValue, $"{fullKey}.{propertyInfo.Name}");
        }
    }

    private static void SetPropertyValue(PropertyInfo propertyInfo, object target, object value)
    {
        var prop = propertyInfo;
        if (!prop.CanWrite)
            prop = propertyInfo.DeclaringType!.GetProperty(propertyInfo.Name);

        if (prop != null && prop.CanWrite)
            prop.SetValue(target, value, null);
    }
}