using System.Diagnostics;
using QuickAcid.Reporting;
using QuickMGenerate;

namespace QuickAcid.Bolts;

public class QRunner
{
    private readonly QAcidScript<Acid> script;
    private readonly RunCount runCount;
    private readonly int? seed;

    public Dictionary<string, int> passedSpecCount { get; } = [];

    public QRunner(QAcidScript<Acid> script, RunCount runCount, int? seed)
    {
        this.script = script;
        this.runCount = runCount;
        this.seed = seed;
    }

    [StackTraceHidden]
    public Report And(ExecutionCount executionCount)
    {
        Report report = null!;
        for (int i = 0; i < runCount.NumberOfRuns; i++)
        {
            var state = seed.HasValue ? new QAcidState(script, seed.Value) : new QAcidState(script);
            report = state.Run(executionCount.NumberOfExecutions);
            state.GetPassedSpecCount(passedSpecCount);
            if (report.IsFailed)
            {
                AddPassedSpecsToReport(report);
                throw new FalsifiableException(report);
            }
        }
        AddPassedSpecsToReport(report);
        return report;
    }

    private void AddPassedSpecsToReport(Report report)
    {
        if (passedSpecCount.Count > 0)
        {
            report.AddEntry(new ReportInfoEntry($" Passed Specs"));
            passedSpecCount.ForEach(kv => report.AddEntry(new ReportInfoEntry($"  - {kv.Key}: {kv.Value}x")));
            report.AddEntry(new ReportInfoEntry(" " + new string('─', 50)));
        }
    }

    [StackTraceHidden]
    public Report AndOneExecutionPerRun() => And(new ExecutionCount(1));
}