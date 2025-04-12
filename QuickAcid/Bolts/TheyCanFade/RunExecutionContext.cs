using QuickAcid.MonadiXEtAl;

namespace QuickAcid.Bolts.TheyCanFade;

public class RunExecutionContext
{
    public Access memory;
    private readonly ShrinkableInputsTrackerPerExecution shrinkTracker;

    public RunExecutionContext(Access memory, ShrinkableInputsTrackerPerExecution shrinkTracker)
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

    public void SetIfNotAlreadyThere<T>(string key, T value) => memory.SetIfNotAlreadyThere(key, value);

    public void AddException(Exception ex)
    {
        memory.LastException = ex;
    }
}
