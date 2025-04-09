// namespace QuickAcid.Bolts.Nuts
// {
//     public static partial class QAcid
//     {
//         public static QAcidRunner<T> LocalVar<T>(this string key, Func<T> func)
//         {
//             //var memory = new Dictionary<string, object>();
//             var val = func();
//             return
//                 s =>
//                 {
//                     // if (s.Reporting || s.Shrinking || s.Verifying)
//                     // {
//                     //     var value1 = memory[key];
//                     //     // var value1 = s.Memory.ForThisAction().Get<T>(key);
//                     //     return new QAcidResult<T>(s, (T)value1) { Key = key };
//                     // }
//                     // var value2 = func();
//                     // memory[key] = value2!;
//                     // //s.Memory.ForThisAction().Set(key, value2);
//                     return new QAcidResult<T>(s, val) { Key = key };
//                 };
//         }
//     }
// }