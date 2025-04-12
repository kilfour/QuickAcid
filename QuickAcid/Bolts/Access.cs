using QuickAcid.MonadiXEtAl;
using QuickAcid.Reporting;

namespace QuickAcid.Bolts;

public class Access
{
    public string? ActionKey { get; set; }

    public Exception? LastException { get; set; }

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

    public void SetIfNotAlreadyThere<T>(object key, T value)
    {
        if (dictionary.ContainsKey(key)) return;
        dictionary[key] = new DecoratedValue { Value = value! };
    }

    public void Set<T>(object key, T value)
    {
        if (!dictionary.ContainsKey(key))
            dictionary[key] = new DecoratedValue { Value = value! };
        else
            dictionary[key].Value = value!;
    }

    public void SetShrinkOutcome(string key, ShrinkOutcome outcome)
    {
        if (dictionary.TryGetValue(key, out var decorated))
        {
            decorated.ShrinkOutcome = outcome;
        }
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
