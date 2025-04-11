namespace QuickAcid.Bolts;

public class AlwaysReportedInputMemory
{
    private Dictionary<string, object> AlwaysReportedInputValues = [];

    private Dictionary<int, Dictionary<string, string>> AlwaysReportedInputReportsPerExecution = [];

    private readonly Func<int> getCurrentActionId;

    public AlwaysReportedInputMemory(Func<int> getCurrentActionId)
    {
        this.getCurrentActionId = getCurrentActionId;
    }

    public T GetOrAdd<T>(string key, Func<T> factory, Func<T, string> stringify)
    {
        if (!AlwaysReportedInputValues.ContainsKey(key))
            AlwaysReportedInputValues[key] = factory()!;
        var value = (T)AlwaysReportedInputValues[key]!;
        ForThisExecution().Add(key, stringify(value));
        return factory();
    }

    public Dictionary<string, string> ForThisExecution()
    {
        if (!AlwaysReportedInputReportsPerExecution.ContainsKey(getCurrentActionId()))
            AlwaysReportedInputReportsPerExecution[getCurrentActionId()] = [];
        return AlwaysReportedInputReportsPerExecution[getCurrentActionId()];
    }
}
