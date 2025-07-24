
using System.Reflection;
using QuickAcid;
using QuickAcid.Bolts;
using QuickAcid.Bolts.ShrinkStrats.Objects;
using QuickAcid.Shrinking.Custom;
using QuickAcid.TheyCanFade;
using QuickFuzzr;
using QuickFuzzr.UnderTheHood;

namespace QuickAcid.Shrinking.Objects;

public class PowersetPropertyNullingStrategy : IObjectShrinkStrategy
{
    public void Shrink<T>(QAcidState state, string key, T value, string fullKey)
    {
        var shrunk = state.Memory.ForThisExecution().GetDecorated(key).GetShrinkOutcome();
        if (shrunk == ShrinkOutcome.Irrelevant)
        {
            var oldValues = new Dictionary<string, object>();
            var propertyInfos = value!.GetType().GetProperties(MyBinding.Flags).ToList();
            foreach (var propertyInfo in propertyInfos)
            {
                oldValues[propertyInfo.Name] = propertyInfo.GetValue(value)!;
            }
            const int MaxPropertyCountForPowerset = 8;
            if (propertyInfos.Count < MaxPropertyCountForPowerset)
            {
                foreach (var set in GetPowerSet(propertyInfos))
                {
                    if (shrunk != ShrinkOutcome.Irrelevant)
                        break;

                    foreach (var propertyInfo in set)
                    {
                        SetPropertyValue(propertyInfo, value, null!);
                    }

                    if (state.VerifyIf.RunPassed(key, value))
                    {
                        foreach (var propertyInfo in set)
                        {
                            state.CurrentExecutionContext().Trace(key, ShrinkKind.PrimitiveKind, new ShrinkTrace
                            {
                                ExecutionId = -1,
                                Key = $"{fullKey}.{propertyInfo.Name}",
                                Name = propertyInfo.Name,
                                Original = oldValues[propertyInfo.Name],
                                Result = oldValues[propertyInfo.Name],
                                Intent = ShrinkIntent.Keep,
                                Strategy = "PrimitiveShrink"
                            });
                        }
                        break;
                    }

                    foreach (var propertyInfo in set)
                    {
                        SetPropertyValue(propertyInfo, value, oldValues[propertyInfo.Name]);
                    }
                }
            }
        }
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

    private static IEnumerable<IEnumerable<T>> GetPowerSet<T>(List<T> list)
    {
        return
            from m in Enumerable.Range(0, 1 << list.Count)
            select
                from i in Enumerable.Range(0, list.Count)
                where (m & 1 << i) != 0
                select list[i];
    }
}