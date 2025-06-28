
using System.Reflection;
using QuickAcid.Bolts;
using QuickAcid.Bolts.TheyCanFade;
using QuickMGenerate;
using QuickMGenerate.UnderTheHood;

namespace QuickAcid.Bolts.ShrinkStrats;

// public class ObjectShrinkStrategy //: IShrinkStrategy
// {
//     public ShrinkOutcome Shrink<T>(QAcidState state, string key, T value)
//     {
//         var shrunk = HandleProperties(state, key, value);

//         if (shrunk == ShrinkOutcome.Irrelevant)
//         {
//             var oldValues = new Dictionary<string, object>();
//             var propertyInfos = value.GetType().GetProperties(MyBinding.Flags).ToList();
//             foreach (var propertyInfo in propertyInfos)
//             {
//                 oldValues[propertyInfo.Name] = propertyInfo.GetValue(value);
//             }
//             const int MaxPropertyCountForPowerset = 8;
//             if (propertyInfos.Count < MaxPropertyCountForPowerset)
//             {
//                 foreach (var set in GetPowerSet(propertyInfos))
//                 {
//                     if (shrunk != ShrinkOutcome.Irrelevant)
//                         break;

//                     foreach (var propertyInfo in set)
//                     {
//                         SetPropertyValue(propertyInfo, value, null);
//                     }

//                     if (!state.ShrinkRunReturnTrueIfFailed(key, value))
//                     {
//                         shrunk = ShrinkOutcome.Report("{ " + string.Join(", ", set.Select(x => $"{x.Name} : {QuickAcidStringify.Default()(oldValues[x.Name])}")) + " }");
//                     }

//                     foreach (var propertyInfo in set)
//                     {
//                         SetPropertyValue(propertyInfo, value, oldValues[propertyInfo.Name]);
//                     }
//                 }
//             }
//         }
//         return shrunk;
//     }

//     private static ShrinkOutcome HandleProperties<T>(QAcidState state, string key, T value)
//     {
//         var messages = value.GetType()
//             .GetProperties(MyBinding.Flags)
//             .Select(p => HandleProperty(state, key, value, p))
//             .OfType<ShrinkOutcome.ReportedOutcome>()
//             .Select(r => r.Message)
//             .ToList();

//         return messages.Any()
//             ? ShrinkOutcome.Report("{ " + string.Join(", ", messages) + " }")
//             : ShrinkOutcome.Irrelevant;
//     }

//     private static ShrinkOutcome HandleProperty(QAcidState state, string key, object value, PropertyInfo propertyInfo)
//     {
//         var propertyValue = propertyInfo.GetValue(value);
//         var primitiveKey = PrimitiveShrinkStrategy.PrimitiveValues.Keys.FirstOrDefault(k => k.IsAssignableFrom(propertyInfo.PropertyType));
//         if (primitiveKey != null)
//         {
//             foreach (var primitiveValue in PrimitiveShrinkStrategy.PrimitiveValues[primitiveKey])
//             {
//                 SetPropertyValue(propertyInfo, value, primitiveValue);
//                 if (!state.ShrinkRunReturnTrueIfFailed(key, value))
//                 {
//                     SetPropertyValue(propertyInfo, value, propertyValue);
//                     return ShrinkOutcome.Report($"{propertyInfo.Name} : {QuickAcidStringify.Default()(propertyValue)}");
//                 }
//             }
//             SetPropertyValue(propertyInfo, value, propertyValue);
//         }
//         return ShrinkOutcome.Irrelevant;
//     }

//     private static void SetPropertyValue(PropertyInfo propertyInfo, object target, object value)
//     {
//         var prop = propertyInfo;
//         if (!prop.CanWrite)
//             prop = propertyInfo.DeclaringType.GetProperty(propertyInfo.Name);

//         if (prop != null && prop.CanWrite) // todo check this
//             prop.SetValue(target, value, null);
//     }

//     private static IEnumerable<IEnumerable<T>> GetPowerSet<T>(List<T> list)
//     {
//         return from m in Enumerable.Range(0, 1 << list.Count)
//                select
//                    from i in Enumerable.Range(0, list.Count)
//                    where (m & 1 << i) != 0
//                    select list[i];
//     }
// }

public class ObjectShrinkStrategy //: IShrinkStrategy
{
    public void Shrink<T>(QAcidState state, string key, T value, string fullKey)
    {

        state.Trace(key, ShrinkKind.ObjectKind, new ShrinkTrace
        {
            Key = fullKey,
            Name = fullKey.Split(".").Last(),
            Original = value,
            Result = value,
            Intent = ShrinkIntent.Keep,
            Strategy = "ObjectShrinkStrategy"
        });


        HandleProperties(state, key, value, fullKey);

        // if (shrunk == ShrinkOutcome.Irrelevant)
        // {
        //     var oldValues = new Dictionary<string, object>();
        //     var propertyInfos = value.GetType().GetProperties(MyBinding.Flags).ToList();
        //     foreach (var propertyInfo in propertyInfos)
        //     {
        //         oldValues[propertyInfo.Name] = propertyInfo.GetValue(value);
        //     }
        //     const int MaxPropertyCountForPowerset = 8;
        //     if (propertyInfos.Count < MaxPropertyCountForPowerset)
        //     {
        //         foreach (var set in GetPowerSet(propertyInfos))
        //         {
        //             if (shrunk != ShrinkOutcome.Irrelevant)
        //                 break;

        //             foreach (var propertyInfo in set)
        //             {
        //                 SetPropertyValue(propertyInfo, value, null);
        //             }

        //             if (!state.ShrinkRunReturnTrueIfFailed(key, value))
        //             {
        //                 shrunk = ShrinkOutcome.Report("{ " + string.Join(", ", set.Select(x => $"{x.Name} : {QuickAcidStringify.Default()(oldValues[x.Name])}")) + " }");
        //             }

        //             foreach (var propertyInfo in set)
        //             {
        //                 SetPropertyValue(propertyInfo, value, oldValues[propertyInfo.Name]);
        //             }
        //         }
        //     }
        // }
        // return shrunk;
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

    private static void HandleProperty(QAcidState state, string key, object value, PropertyInfo propertyInfo, string fullKey)
    {
        var propertyValue = propertyInfo.GetValue(value);
        var swapper = new MemoryLens(
            obj => propertyInfo.GetValue(obj)!,
            (val, propValue) => { SetPropertyValue(propertyInfo, value, propValue); return val; });
        using (state.Memory.NestedValue(swapper))
        {
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
        return from m in Enumerable.Range(0, 1 << list.Count)
               select
                   from i in Enumerable.Range(0, list.Count)
                   where (m & 1 << i) != 0
                   select list[i];
    }
}