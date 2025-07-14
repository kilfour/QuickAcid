using System.Diagnostics;
using QuickAcid.Bolts;
using QuickAcid.Reporting;
using QuickMGenerate;

namespace QuickAcid;

public class QRunner
{
    private readonly QAcidScript<Acid> script;
    private readonly RunCount runCount;

    public Dictionary<string, int> passedSpecCount { get; } = [];

    public QRunner(QAcidScript<Acid> script, RunCount runCount)
    {
        this.script = script;
        this.runCount = runCount;
    }

    [StackTraceHidden]
    public void And(ExecutionCount executionCount)
    {
        runCount.NumberOfRuns.Times(() =>
        {
            var state = new QAcidState(script);
            var report = state.Run(executionCount.NumberOfExecutions);
            state.GetPassedSpecCount(passedSpecCount);
            if (report != null)
            {
                passedSpecCount.ForEach(kv => report.AddEntry(new ReportInfoEntry($"  {kv.Key}: {kv.Value}")));
                throw new FalsifiableException(report);
            }
        });
    }

    public void AndOneExecutionPerRun() => And(new ExecutionCount(1));
}