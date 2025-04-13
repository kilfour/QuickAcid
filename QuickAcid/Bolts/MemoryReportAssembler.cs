using QuickAcid.Bolts.TheyCanFade;
using QuickAcid.Reporting;

namespace QuickAcid.Bolts;


public static class MemoryReportAssembler
{
    public static void AddAllMemoryToReport(
        QAcidReport report,
        Memory memory,
        int executionId,
        Exception exception, bool isFinalRun)
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
                    if (val.ShrinkOutcome is ShrinkOutcome.ReportedOutcome(var msg))
                    {
                        report.AddEntry(new ReportInputEntry(key) { Value = msg });
                    }
                    else if (!isFinalRun)
                    {
                        if (val.ReportingIntent != ReportingIntent.Never)
                            report.AddEntry(new ReportInputEntry(key) { Value = QuickAcidStringify.Default()(val.Value) });
                    }
                }
                return Acid.Test;
            },
            none: () => Acid.Test);
    }
}


