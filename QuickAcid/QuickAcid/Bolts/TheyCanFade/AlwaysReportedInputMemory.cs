using QuickAcid.MonadiXEtAl;
using QuickAcid.Reporting;
using QuickPulse.Diagnostics;

namespace QuickAcid.Bolts.TheyCanFade;

public class AlwaysReportedInputMemory
{
    private readonly Func<int> getCurrentActionId;

    private readonly Dictionary<string, object> values = [];
    private readonly Dictionary<int, Dictionary<string, string>> reportPerExecution = [];

    public AlwaysReportedInputMemory(Func<int> getCurrentActionId)
    {
        this.getCurrentActionId = getCurrentActionId;
    }

    public T Store<T>(string key, Func<T> factory, Func<T, string> stringify)
    {
        QAcidState.GetPulse(["AlwaysReportedInputMemory"])($"Store({key} ...)");
        var val = StoreWithoutReporting(key, factory);
        ReportForCurrent()[key] = stringify(val);
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

    public Maybe<T> Retrieve<T>(string key)
    {
        return values.TryGetValue(key, out var value) ? Maybe<T>.Some((T)value!) : Maybe<T>.None;
    }

    public void Reset()
    {
        QAcidState.GetPulse(["AlwaysReportedInputMemory"])($"Reset()");
        values.Clear();
        reportPerExecution.Clear();
    }

    public void AddToReport(QAcidReport report, int executionId)
    {
        if (reportPerExecution.TryGetValue(executionId, out var dict))
        {
            foreach (var kv in dict)
            {
                report.AddEntry(new ReportAlwaysReportedInputEntry(kv.Key)
                {
                    Value = kv.Value
                });
            }
        }
    }

    private Dictionary<string, string> ReportForCurrent()
    {
        var id = getCurrentActionId();
        if (!reportPerExecution.ContainsKey(id))
            reportPerExecution[id] = [];
        return reportPerExecution[id];
    }

    public IReadOnlyDictionary<int, Dictionary<string, string>> ReportPerExecutionSnapshot()
    {
        // If you're okay exposing the actual dictionary (it's not modified externally):
        return reportPerExecution;

        // OR: if you'd rather clone it to avoid accidental mutation:
        // return reportPerExecution.ToDictionary(
        //     entry => entry.Key,
        //     entry => new Dictionary<string, string>(entry.Value));
    }

    public IEnumerable<string> GetAllAlwaysReportedKeys()
    {
        return values.Keys;
    }

    // ---------------------------------------------------------------------------------------
    // -- DEEP COPY
    public AlwaysReportedInputMemory DeepCopy(Func<int> getCurrentActionId)
    {
        var newValues = new Dictionary<string, object>(values); // shallow copy of values
        var newReports = new Dictionary<int, Dictionary<string, string>>();
        foreach (var kvp in reportPerExecution)
        {
            var copiedInner = new Dictionary<string, string>(kvp.Value);
            newReports[kvp.Key] = copiedInner;
        }
        return new AlwaysReportedInputMemory(getCurrentActionId, newValues, newReports);
    }

    public AlwaysReportedInputMemory(
        Func<int> getCurrentActionId,
        Dictionary<string, object> values,
        Dictionary<int, Dictionary<string, string>> reportPerExecution)
    {
        this.getCurrentActionId = getCurrentActionId;
        this.values = values;
        this.reportPerExecution = reportPerExecution;
    }
    // ---------------------------------------------------------------------------------------
}