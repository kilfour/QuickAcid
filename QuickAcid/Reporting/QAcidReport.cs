using System.Text;

namespace QuickAcid.Reporting;

public class QAcidReport
{
    private readonly List<IAmAReportEntry> entries = [];

    public List<IAmAReportEntry> Entries { get { return entries; } }

    public Exception? Exception { get; set; }
    public virtual void AddEntry(IAmAReportEntry reportEntry)
    {
        entries.Add(reportEntry);
    }

    public override string ToString()
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine();
        stringBuilder.AppendLine(" ---------------------------");
        foreach (var entry in entries)
        {
            stringBuilder.AppendLine(entry.ToString());
        }
        return stringBuilder.ToString();
    }

    public bool FailedWith<T>()
    {
        return Exception?.GetType() == typeof(T);
    }

    public bool FailedWith(string specKey)
    {
        return entries.OfType<QAcidReportSpecEntry>().SingleOrDefault()?.Key == specKey;
    }

    public IEnumerable<T> OfType<T>()
    {
        return entries.OfType<T>();
    }

    public T? FirstOrDefault<T>()
    {
        return OfType<T>().FirstOrDefault();
    }

    public T? SecondOrDefault<T>()
    {
        var arr = OfType<T>().ToArray();
        if (arr.Length < 2) return default;
        return arr[1];
    }

    public T Single<T>()
    {
        return OfType<T>().Single();
    }
}
