using QuickAcid.MonadiXEtAl;
using QuickAcid.Reporting;

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
        if (!values.ContainsKey(key))
        {
            var value = factory();
            values[key] = value!;
        }

        var val = (T)values[key]!;
        ReportForCurrent()[key] = stringify(val);
        return val;
    }

    public Maybe<T> Retrieve<T>(string key)
    {
        return values.TryGetValue(key, out var value) ? Maybe<T>.Some((T)value!) : Maybe<T>.None;
    }

    public void Reset()
    {
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
}