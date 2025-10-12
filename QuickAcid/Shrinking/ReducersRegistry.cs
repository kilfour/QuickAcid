using QuickAcid.Bolts;
using QuickAcid.TheyCanFade;

namespace QuickAcid.Shrinking;

public interface IReducer<T>
{
    IEnumerable<T> Reduce(T value);
}

public class LambdaReducer<T> : IReducer<T>
{
    private readonly Func<T, IEnumerable<T>> shrinker;
    public LambdaReducer(Func<T, IEnumerable<T>> shrinker) => this.shrinker = shrinker;
    public IEnumerable<T> Reduce(T value) => shrinker(value);
}

public class ReducersRegistry
{
    private readonly Dictionary<Type, object> reducers = [];

    public void Register<T>(Func<T, IEnumerable<T>> reducer)
        => reducers[typeof(T)] = new LambdaReducer<T>(reducer);

    public void Remove<T>()
        => reducers.Remove(typeof(T));

    public IReducer<T>? TryGet<T>()
    {
        return reducers.TryGetValue(typeof(T), out var reducer)
            ? reducer as IReducer<T>
            : null;
    }
}

public class CustomReductionStrategy<T>
{
    private readonly IReducer<T> reducer;

    public CustomReductionStrategy(IReducer<T> reducer)
    {
        this.reducer = reducer;
    }

    public void Reduce(QAcidState state, string key, object original)
    {
        T passed = default!;
        var lastFailed = (T)original;
        var values = reducer.Reduce(lastFailed);
        var found = false;
        foreach (var candidate in values)
        {
            if (state.VerifyIf.RunPassed(key, candidate!))
            {
                passed = candidate;
                found = true;
            }
            else
            {
                lastFailed = candidate;
            }
        }
        if (found)
            state.CurrentExecutionContext().ShrinkTrace(key, ShrinkKind.Unknown, new ShrinkTrace
            {
                ExecutionId = -1,
                Key = key,
                Name = key.Split(".").Last(),
                Original = original,
                Result = lastFailed,
                Intent = ShrinkIntent.Replace,
                Strategy = "CustomReductionStrategy"
            });
        else
            state.CurrentExecutionContext().ShrinkTrace(key, ShrinkKind.Unknown, new ShrinkTrace
            {
                ExecutionId = -1,
                Key = key,
                Name = key.Split(".").Last(),
                Original = original,
                Result = original,
                Intent = ShrinkIntent.Keep,
                Strategy = "CustomReductionStrategy"
            });

        return;
    }
}