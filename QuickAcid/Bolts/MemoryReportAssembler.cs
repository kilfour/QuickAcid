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
                    // 🧠📉 REPORT FILTERING LOGIC COMMENT – DO NOT DELETE:
                    // Only values with a shrink outcome are considered reportable.
                    // This includes explicitly reported shrinks and irrelevant-but-shrunk markers.
                    // Values without any ShrinkOutcome are deliberately excluded from the report.
                    // They are NOT broken — just not participating in reporting.
                    switch (val.ShrinkOutcome)
                    {
                        case ShrinkOutcome.IrrelevantOutcome:
                            continue;
                        case ShrinkOutcome.ReportedOutcome(var msg):
                            report.AddEntry(new ReportInputEntry(key) { Value = msg });
                            break;
                    }
                }
                return Acid.Test;
            },
            none: () => Acid.Test);
    }
}


