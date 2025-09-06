using QuickAcid.Shrinking;

namespace QuickAcid.TheyCanFade;

public class RunExecutionContext(
    int executionNumber,
    Access access,
    InputTrackerPerExecution shrinkTracker,
    Dictionary<string, string> traces)
{
    private readonly int executionNumber = executionNumber;
    private readonly Access access = access;
    private readonly InputTrackerPerExecution shrinkTracker = shrinkTracker;
    private readonly Dictionary<string, string> traces = traces;

    public bool AlreadyTriedToShrink(string key) => shrinkTracker.AlreadyTried(key);
    public void MarkAsTriedToShrink(string key) => shrinkTracker.MarkAsTriedToShrink(key);

    public bool ContainsActionKey(string key) => access.ActionKeys.Contains(key);
    public void AddActionKey(string key) => access.ActionKeys.Add(key);
    public bool ContainsKey(string key) => access.ContainsKey(key);
    public T Get<T>(string key) => access.Get<T>(key);

    public void SetIfNotAlreadyThere<T>(string key, T value) => access.SetIfNotAlreadyThere(key, value);

    public string Trace(string key, string trace)
    {
        traces[key] = trace;
        return trace;
    }

    public void Trace(string key, ShrinkKind shrinkKind, ShrinkTrace trace)
    {
        access.GetDecorated(key).AddTrace(shrinkKind, trace with { ExecutionId = executionNumber });
    }

    public T Remember<T>(string key, Func<T> factory, ReportingIntent reportingIntent = ReportingIntent.Shrinkable)
    {
        if (!access.ContainsKey(key))
        {
            var value = factory();
            access.Set(key, value, reportingIntent);
            return value;
        }
        return Get<T>(key);
    }
}
