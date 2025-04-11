using QuickAcid.MonadiXEtAl;
using QuickAcid.Reporting;

namespace QuickAcid.Bolts;

public class Access
{
    public string? ActionKey { get; set; }

    public Exception? LastException { get; set; }
    public bool IsIrrelevant { get; set; }
    private Dictionary<object, DecoratedValue> dictionary = [];

    // only for alwaysReported
    public T GetOrAdd<T>(object key, Func<T> factory, Func<T, string> stringify)
    {
        if (!dictionary.ContainsKey(key))
            dictionary[key] = new DecoratedValue { Value = factory()!, IsIrrelevant = false, Stringify = obj => stringify((T)obj) };
        return Get<T>(key);
    }

    public T Get<T>(object key)
    {
        return (T)dictionary[key].Value!;
    }

    public Maybe<T> GetMaybe<T>(object key)
    {
        return dictionary.TryGetValue(key, out var value)
            ? Maybe<T>.Some((T)value.Value!)
            : Maybe<T>.None;
    }

    public void SetIfNotAllReadyThere<T>(object key, T value)
    {
        if (dictionary.ContainsKey(key)) return;
        dictionary[key] = new DecoratedValue { Value = value!, IsIrrelevant = false };
    }

    public void Set<T>(object key, T value)
    {
        if (!dictionary.ContainsKey(key))
            dictionary[key] = new DecoratedValue { Value = value!, IsIrrelevant = false };
        else
            dictionary[key].Value = value!;
    }

    public void MarkAsIrrelevant<T>(object key)
    {
        dictionary[key].IsIrrelevant = true;
    }

    public void AddReportingMessage<T>(object key, string message)
    {
        dictionary[key].ReportingMessage = message;
    }

    public bool ContainsKey(object key)
    {
        return dictionary.ContainsKey(key);
    }

    public Dictionary<string, DecoratedValue> GetAll()
    {
        return dictionary
            .Where(kvp => kvp.Key is string)
            .ToDictionary(kvp => (string)kvp.Key, kvp => kvp.Value);
    }

    public void AddToReport(QAcidReport report, Exception exceptionFromState)
    {
        bool isSameException = LastException?.ToString() == exceptionFromState?.ToString();

        report.AddEntry(
            new ReportActEntry(ActionKey!)
            {
                Exception = isSameException ? LastException : null
            });

        foreach (var pair in GetAll())
        {
            if (pair.Value!.IsIrrelevant) continue;
            var value = string.IsNullOrEmpty(pair.Value.ReportingMessage) ? pair.Value.Value : pair.Value.ReportingMessage;
            report.AddEntry(new ReportInputEntry(pair.Key) { Value = value });
        }
    }
}
