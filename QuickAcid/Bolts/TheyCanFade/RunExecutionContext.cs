using QuickAcid.Bolts.ShrinkStrats;

namespace QuickAcid.Bolts.TheyCanFade;

public class RunExecutionContext
{
    private readonly int executionNumber;
    public Access access;
    private readonly InputTrackerPerExecution shrinkTracker;
    private readonly Dictionary<string, string> traces;

    public RunExecutionContext(
        int executionNumber,
        Access access,
        InputTrackerPerExecution shrinkTracker,
        Dictionary<string, string> traces)
    {
        this.executionNumber = executionNumber;
        this.access = access;
        this.shrinkTracker = shrinkTracker;
        this.traces = traces;
    }

    public bool AlreadyTriedToShrink(string key) => shrinkTracker.AlreadyTried(key);

    public void MarkAsTriedToShrink(string key)
    {
        shrinkTracker.MarkAsTriedToShrink(key);
    }

    public T Get<T>(string key) => access.Get<T>(key);
    public DecoratedValue GetDecorated(string key) => access.GetDecorated(key);

    public void SetIfNotAlreadyThere<T>(string key, T value) => access.SetIfNotAlreadyThere(key, value);

    public string Trace(string key, string trace)
    {
        traces[key] = trace;
        return trace;
    }

    public void SetShrinkKind(string key, ShrinkKind shrinkKind)
    {
        access.GetDecorated(key).SetShrinkKind(shrinkKind);
    }
    public void Trace(string key, ShrinkKind shrinkKind, ShrinkTrace trace)
    {
        access.GetDecorated(key).AddTrace(shrinkKind, trace with { ExecutionId = executionNumber });
    }
}
