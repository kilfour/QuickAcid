namespace QuickAcid.Tests.Needling;

public static class TaskExtensions
{
    public static Task<TOut> Attach<TIn, TOut>(this Task<TOut> task, Needler<TIn, TOut> needler, string key, TIn record)
    {
        task.ContinueWith(a => needler.Register(key, record, a.Result), TaskContinuationOptions.OnlyOnRanToCompletion);
        return task;
    }
}
// public static Task Attach<T>(this Task task, Needler<T> needler, string key, T record)
// {
//     task.ContinueWith(a => needler.Register(key, record));
//     return task;
// }

// t.ContinueWith(_ => needler.TryAdd(name, 1),
//     TaskContinuationOptions.OnlyOnRanToCompletion, TaskScheduler.Default);

// t.ContinueWith(x => errors.Add(x.Exception!),
//     TaskContinuationOptions.OnlyOnFaulted, TaskScheduler.Default);

// Make sure background tasks finish and no errors slipped by
// from _ in "Drain background work".Act(() =>
// {
//     var tasks = running.ToArray();
//     if (tasks.Length > 0) Task.WaitAll(tasks);
//     if (!errors.IsEmpty) throw new AggregateException(errors);
// })

// Pseudocode-ish QuickAcid combinator
// public static QAcidScript<Acid> Eventually(
//     this QAcidScript<Acid> inner,
//     TimeSpan deadline,
//     TimeSpan poll)
// {
//     return state =>
//     {
//         var stop = DateTime.UtcNow + deadline;
//         while (DateTime.UtcNow < stop)
//         {
//             var r = inner(state);
//             if (r.IsSuccess) return r; // observed
//             Thread.Sleep(poll);         // advance time without awaiting a specific task
//         }
//         return Acid.Spec("Eventually", false)(state);
//     };
// }
