namespace QuickAcid.TheyCanFade;

public class Access
{
    public HashSet<string> ActionKeys { get; set; } = [];

    private readonly Dictionary<object, DecoratedValue> dictionary = [];

    public bool ContainsKey(object key)
    {
        return dictionary.ContainsKey(key);
    }

    public T Get<T>(object key)
    {
        return (T)dictionary[key].Value!;
    }

    public DecoratedValue GetDecorated(object key)
    {
        return dictionary[key];
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

    public Dictionary<string, DecoratedValue> GetAll()
    {
        return dictionary
            .Where(kvp => kvp.Key is string)
            .ToDictionary(kvp => (string)kvp.Key, kvp => kvp.Value);
    }
}
