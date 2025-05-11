using QuickAcid.Bolts.ShrinkStrats;
using QuickAcid.MonadiXEtAl;

namespace QuickAcid.Bolts.TheyCanFade;

public class Access
{
    public HashSet<string> ActionKeys { get; set; } = [];

    private Dictionary<object, DecoratedValue> dictionary = [];

    public T Get<T>(object key)
    {
        return (T)dictionary[key].Value!;
    }

    public DecoratedValue GetDecorated(object key)
    {
        return dictionary[key];
    }

    public string GetAsString(object key) // for codegen
    {
        return dictionary[key]?.Value?.ToString()!;
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

    public void Override<T>(object key, T value)
    {
        dictionary[key] = new DecoratedValue { Value = value! };
    }

    public void Set<T>(object key, T value, ReportingIntent reportingIntent)
    {
        if (!dictionary.ContainsKey(key))
            dictionary[key] = new DecoratedValue { Value = value!, ReportingIntent = reportingIntent };
        else
        {
            dictionary[key].Value = value!;
            dictionary[key].ReportingIntent = reportingIntent;
        }
    }

    public void SetReportingIntent<T>(object key, ReportingIntent reportingIntent)
    {
        dictionary[key].ReportingIntent = reportingIntent;
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

    // ---------------------------------------------------------------------------------------
    // -- DEEP COPY
    public Access DeepCopy()
    {
        var newAccess = new Access
        {
            ActionKeys = [.. ActionKeys]
        };
        foreach (var kvp in dictionary)
        {
            newAccess.Add(kvp.Key, kvp.Value.DeepCopy());
        }
        return newAccess;
    }

    public void Add(object key, DecoratedValue value)
    {
        dictionary[key] = value;
    }
    // ---------------------------------------------------------------------------------------
}
