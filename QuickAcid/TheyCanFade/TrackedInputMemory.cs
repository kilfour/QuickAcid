using QuickPulse.Show;

namespace QuickAcid.TheyCanFade;

public class TrackedInputMemory
{
    private readonly Func<int> getCurrentExecutionId;

    private readonly Dictionary<string, object> values = [];
    private readonly Dictionary<int, Dictionary<string, string>> reportPerExecution = [];

    public TrackedInputMemory(Func<int> getCurrentExecutionId)
    {
        this.getCurrentExecutionId = getCurrentExecutionId;
    }

    public T Store<T>(string key, Func<T> factory)
    {
        var val = StoreWithoutReporting(key, factory);
        ReportForCurrent()[key] = Introduce.This(val!, false);
        return val;
    }

    public T StoreWithoutReporting<T>(string key, Func<T> factory)
    {
        if (!values.ContainsKey(key))
        {
            var value = factory();
            values[key] = value!;
        }
        var val = (T)values[key]!;
        return val;
    }

    public void Reset()
    {
        values.Clear();
        reportPerExecution.Clear();
    }

    private Dictionary<string, string> ReportForCurrent()
    {
        var id = getCurrentExecutionId();
        if (!reportPerExecution.ContainsKey(id))
            reportPerExecution[id] = [];
        return reportPerExecution[id];
    }

    public IReadOnlyDictionary<int, Dictionary<string, string>> TrackedInputsPerExecution()
    {
        return reportPerExecution;
    }

    public IEnumerable<string> GetAllTrackedKeys()
    {
        return values.Keys;
    }
}