using QuickAcid.MonadiXEtAl;

namespace QuickAcid.Bolts;

public class ExecutionContext
{
    private readonly Access memory;
    private readonly ShrinkableInputsTrackerPerExecution shrinkTracker;

    public ExecutionContext(Access memory, ShrinkableInputsTrackerPerExecution shrinkTracker)
    {
        this.memory = memory;
        this.shrinkTracker = shrinkTracker;
    }

    public bool AlreadyTried(string key) => shrinkTracker.AlreadyTried(key);

    public void SetShrinkOutcome(string key, ShrinkOutcome outcome)
    {
        shrinkTracker.MarkAsTriedToShrink(key);
        if (memory.ContainsKey(key))
        {
            memory.SetShrinkOutcome(key, outcome);
        }
    }

    public T Get<T>(string key) => memory.Get<T>(key);

    public Maybe<T> GetMaybe<T>(string key) => memory.GetMaybe<T>(key);

    public void SetIfAbsent<T>(string key, T value) => memory.SetIfNotAllReadyThere(key, value);
}
