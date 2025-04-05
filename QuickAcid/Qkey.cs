// // QKey.cs

// namespace QuickAcid
// {
//     public readonly struct QKey<T>
//     {
//         public string Label { get; }

//         public QKey(string label)
//         {
//             Label = label;
//         }

//         public override string ToString() => Label;
//     }

//     public static class QKeys
//     {
//         // Optional: predefined, commonly used keys for convenience
//         public static readonly QKey<int> X = new("x");
//         public static readonly QKey<int> Y = new("y");
//         public static readonly QKey<string> User = new("user");
//         //public static readonly QKey<Tracker> Tracker = new("tracker");
//     }

//     public interface QAcidContext
//     {
//         T Get<T>(QKey<T> key);

//         // Optional legacy fallback
//         T Get<T>(string label);
//     }

//     // Usage example (fluent test):
//     // .TrackedInput(QKeys.Tracker, () => new Tracker())
//     // .Perform("check", ctx => () => ctx.Get(QKeys.Tracker).AssertSomething())
// }

// // In QAcidState (example implementation):
// // public T Get<T>(QKey<T> key) => Memory.Get<T>(key.Label);
