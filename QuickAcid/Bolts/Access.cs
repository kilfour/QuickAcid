using QuickAcid.MonadiXEtAl;
using QuickAcid.Reporting;

namespace QuickAcid.Bolts;

public class Access
{
    public string? ActionKey { get; set; }

    public Exception? LastException { get; set; }
    public bool IsIrrelevant { get; set; }
    private Dictionary<object, DecoratedValue> dictionary = [];

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
}
