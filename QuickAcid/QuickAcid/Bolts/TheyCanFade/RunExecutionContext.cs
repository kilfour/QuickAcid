using QuickAcid.Bolts.ShrinkStrats;
using QuickAcid.MonadiXEtAl;

namespace QuickAcid.Bolts.TheyCanFade;

public class RunExecutionContext
{
    public Access memory;
    private readonly InputTrackerPerExecution shrinkTracker;

    private readonly Dictionary<string, string> traces;

    public RunExecutionContext(Access memory, InputTrackerPerExecution shrinkTracker, Dictionary<string, string> traces)
    {
        this.memory = memory;
        this.shrinkTracker = shrinkTracker;
        this.traces = traces;
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
    public DecoratedValue GetDecorated(string key) => memory.GetDecorated(key);

    public Maybe<T> GetMaybe<T>(string key) => memory.GetMaybe<T>(key);

    public void SetIfNotAlreadyThere<T>(string key, T value) => memory.SetIfNotAlreadyThere(key, value);

    public string Trace(string key, string trace)
    {
        traces[key] = trace;
        return trace;
    }
}
