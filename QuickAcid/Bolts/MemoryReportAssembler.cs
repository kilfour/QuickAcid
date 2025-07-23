using QuickAcid.Bolts.ShrinkStrats;
using QuickAcid.Bolts.TheyCanFade;
using QuickAcid.Reporting;
using QuickPulse.Show;

namespace QuickAcid.Bolts;

public static class MemoryReportAssembler
{
    public static List<ReportEntry> GetReportEntriesForExecution(
        Memory memory,
        int executionId,
        bool isFinalRun)
    {
        List<ReportEntry> entries = [];
        if (memory.TrackedSnapshot().TryGetValue(executionId, out var snapshot))
        {
            foreach (var (key, val) in snapshot)
                entries.Add(new ReportTrackedEntry(key) { Value = val });
        }

        if (memory.Has(executionId))
        {
            var access = memory.For(executionId);
            var executionKey = string.Join(", ", access.ActionKeys);
            entries.Add(new ReportExecutionEntry(executionKey, executionId));
            foreach (var (key, val) in access.GetAll())
            {
                var shrinkOutcome = val.GetShrinkOutcome();
                if (shrinkOutcome is ShrinkOutcome.ReportedOutcome(var msg))
                {
                    entries.Add(new ReportInputEntry(key) { Value = msg });
                }
                else if (!isFinalRun)
                {
                    if (val.ReportingIntent != ReportingIntent.Never)
                    {
                        entries.Add(new ReportInputEntry(key)
                        {
                            Value = Introduce.This(val.Value!, false)
                        });
                    }
                }
            }
        }

        foreach (var (key, val) in memory.TracesFor(executionId))
        {
            entries.Add(new ReportTraceEntry(key) { Value = val });
        }

        return entries;
    }
}


