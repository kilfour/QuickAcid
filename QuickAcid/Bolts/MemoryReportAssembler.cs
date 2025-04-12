using QuickAcid.Reporting;

namespace QuickAcid.Bolts;


public static class MemoryReportAssembler
{
    public static void AddAllMemoryToReport(
        QAcidReport report,
        Memory memory,
        int executionId,
        Exception exception)
    {
        // Always-reported snapshot
        if (memory.AlwaysReportedSnapshot().TryGetValue(executionId, out var snapshot))
        {
            foreach (var (key, val) in snapshot)
                report.AddEntry(new ReportAlwaysReportedInputEntry(key) { Value = val });
        }

        // Per-action memory
        memory.TryGet(executionId).Match(
            some: access =>
            {
                bool same = access.LastException?.ToString() == exception?.ToString();
                report.AddEntry(new ReportActEntry(access.ActionKey!)
                {
                    Exception = same ? access.LastException : null
                });

                foreach (var (key, val) in access.GetAll())
                {
                    if (val.IsIrrelevant) continue;

                    var output = string.IsNullOrEmpty(val.ReportingMessage)
                        ? val.Value
                        : val.ReportingMessage;

                    report.AddEntry(new ReportInputEntry(key) { Value = output });
                }
                return Acid.Test;
            },
            none: () => Acid.Test);
    }
}


