using System.Dynamic;
using System.Text;
using QuickAcid.Bolts.ShrinkStrats;

namespace QuickAcid.Reporting;

public class Report
{
    private readonly List<IAmAReportEntry> entries = [];
    public List<IAmAReportEntry> Entries { get { return entries; } }
    public Exception? Exception { get; set; }
    public string Code { get; set; } = string.Empty;
    public List<ShrinkTrace> ShrinkTraces { get; set; } = [];

    public virtual void AddEntry(IAmAReportEntry reportEntry)
    {
        entries.Add(reportEntry);
    }

    public override string ToString()
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine("QuickAcid Report:");
        foreach (var entry in entries)
        {
            stringBuilder.AppendLine(entry.ToString());
        }
        if (Exception != null)
            stringBuilder.AppendLine(Exception.ToString());
        if (Code != null)
            stringBuilder.AppendLine(Code);
        return stringBuilder.ToString();
    }

    public bool FailedWith<T>()
    {
        return Exception?.GetType() == typeof(T);
    }

    public bool FailedWith(string specKey)
    {
        return entries.OfType<ReportSpecEntry>().SingleOrDefault()?.Key == specKey;
    }

    public IEnumerable<T> OfType<T>()
    {
        return entries.OfType<T>();
    }

    public T First<T>()
    {
        return OfType<T>().First();
    }

    public T? FirstOrDefault<T>()
    {
        return OfType<T>().FirstOrDefault();
    }

    public T Second<T>()
    {
        var result = SecondOrDefault<T>();
        if (result == null) throw new NullReferenceException();
        return result;
    }

    public T? SecondOrDefault<T>()
    {
        var arr = OfType<T>().ToArray();
        if (arr.Length < 2) return default;
        return arr[1];
    }

    public T Third<T>()
    {
        var result = ThirdOrDefault<T>();
        if (result == null) throw new NullReferenceException();
        return result;
    }

    public T? ThirdOrDefault<T>()
    {
        var arr = OfType<T>().ToArray();
        if (arr.Length < 3) return default;
        return arr[2];
    }

    public T Fourth<T>()
    {
        var result = FourthOrDefault<T>();
        if (result == null) throw new NullReferenceException();
        return result;
    }

    public T? FourthOrDefault<T>()
    {
        var arr = OfType<T>().ToArray();
        if (arr.Length < 4) return default;
        return arr[3];
    }

    public T Single<T>()
    {
        return OfType<T>().Single();
    }

    public ReportSpecEntry GetSpecEntry()
    {
        return Single<ReportSpecEntry>();
    }
}
