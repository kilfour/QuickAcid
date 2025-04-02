using System.Text;

namespace QuickAcid
{
    public class QAcidReport
    {
        private readonly List<QAcidReportEntry> entries = [];

        public List<QAcidReportEntry> Entries { get { return entries; } }

        public int ShrinkAttempts { get; set; }

        public virtual void AddEntry(QAcidReportEntry reportEntry)
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
    }

    public abstract class QAcidReportEntry
    {
        protected readonly string TheKey;
        public string Key => TheKey;

        protected QAcidReportEntry(string key)
        {
            TheKey = key;
        }
    }

    public class QAcidReportActEntry : QAcidReportEntry
    {
        public Exception? Exception { get; set; }

        public QAcidReportActEntry(string key) : base(key) { }

        public QAcidReportActEntry(string key, Exception exception) : this(key)
        {
            Exception = exception;
        }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder($"Execute : {Key}");
            stringBuilder.AppendLine();
            if (Exception != null)
            {
                stringBuilder.Append(Exception);
                stringBuilder.AppendLine();
            }
            stringBuilder.Append(" ---------------------------");
            return stringBuilder.ToString();
        }
    }

    public class QAcidReportInputEntry : QAcidReportEntry
    {
        public object? Value { get; set; }

        public QAcidReportInputEntry(string key)
            : base(key) { }

        public override string ToString()
        {
            if (Value == null)
                return $"Input : {Key}";
            return $"Input : {Key} = {Value}";
        }
    }

    public class QAcidReportSpecEntry : QAcidReportEntry
    {
        public QAcidReportSpecEntry(string key)
            : base(key) { }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine();
            stringBuilder.AppendLine(" ********************************");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine($"  Spec Failed : {Key}");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine(" ********************************");
            return stringBuilder.ToString();
        }
    }
}