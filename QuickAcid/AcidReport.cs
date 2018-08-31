using System;
using System.Collections.Generic;
using System.Text;

namespace QuickAcid
{
    public class AcidReport
    {
        private List<object> entries = new List<object>();

        public void AddEntry(AcidReportEntry reportEntry)
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

    public abstract class AcidReportEntry
    {
        protected readonly string Key;
        protected AcidReportEntry(string key)
        {
            Key = key;
        }
    }

    public class AcidReportActEntry : AcidReportEntry
    {
        public AcidReportActEntry(string key)
            : base(key) { }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder($"Execute : {Key}");
            stringBuilder.AppendLine();
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