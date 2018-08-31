using System;
using System.Collections.Generic;
using System.Text;

namespace QuickAcid
{
    public class AcidReport
    {
        private readonly List<object> entries = new List<object>();

        public List<object> Entries { get { return entries; } }

        public virtual void AddEntry(AcidReportEntry reportEntry)
        {
            entries.Add(reportEntry);
        }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder(" ---------------------------");
            stringBuilder.AppendLine();
            foreach (var entry in entries)
            {
                stringBuilder.AppendLine(entry.ToString());
            }
            return stringBuilder.ToString();
        }
    }

    public class DevNullReport : AcidReport
    {
        public override void AddEntry(AcidReportEntry reportEntry) { }
    }

    public abstract class AcidReportEntry
    {
        protected readonly string TheKey;
        public string Key => TheKey;

        protected AcidReportEntry(string key)
        {
            TheKey = key;
        }
    }

    public class AcidReportActEntry : AcidReportEntry
    {
        public Exception Exception { get; set; }

        public AcidReportActEntry(string key) : base(key)
        {
            
        }

        public AcidReportActEntry(string key, Exception exception) : this(key)
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

    public class AcidReportInputEntry : AcidReportEntry
    {
        public object Value;

        public AcidReportInputEntry(string key)
            : base(key) { }

        public override string ToString()
        {
            return $"Input : {Key} = {Value}";
        }
    }

    public class AcidReportSpecEntry : AcidReportEntry
    {
        public object Value;

        public AcidReportSpecEntry(string key)
            : base(key) { }

        public override string ToString()
        {
            return $"Spec Failed : {Key}";
        }
    }
}