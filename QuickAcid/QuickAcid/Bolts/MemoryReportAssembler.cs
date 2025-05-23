using QuickAcid.Bolts.ShrinkStrats;
using QuickAcid.Bolts.TheyCanFade;
using QuickAcid.Reporting;

namespace QuickAcid.Bolts;

public static class MemoryReportAssembler
{
    public static void AddAllMemoryToReport(
        Report report,
        Memory memory,
        int executionId,
        Exception exception, bool isFinalRun)
    {
        // Always-reported snapshot
        if (memory.TrackedSnapshot().TryGetValue(executionId, out var snapshot))
        {
            foreach (var (key, val) in snapshot)
                report.AddEntry(new ReportTrackedEntry(key) { Value = val });
        }

        // Per-action memory
        memory.TryGet(executionId).Match(
            some: access =>
            {
                report.AddEntry(
                    new ReportExecutionEntry(string.Join(", ",
                    access.ActionKeys.Select(LabelPrettifier.Prettify))));


                foreach (var (key, val) in access.GetAll())
                {
                    if (val.ShrinkOutcome is ShrinkOutcome.ReportedOutcome(var msg))
                    {
                        report.AddEntry(new ReportInputEntry(LabelPrettifier.Prettify(key)) { Value = msg });
                    }
                    else if (!isFinalRun)
                    {
                        if (val.ReportingIntent != ReportingIntent.Never)
                            report.AddEntry(new ReportInputEntry(LabelPrettifier.Prettify(key))
                            {
                                Value = QuickAcidStringify.Default()(val.Value!)
                            });
                    }
                }
                return Acid.Test;
            },
            none: () => Acid.Test);

        // traces
        foreach (var (key, val) in memory.TracesFor(executionId))
        {
            report.AddEntry(new ReportTraceEntry(key) { Value = val });
        }
    }
}


