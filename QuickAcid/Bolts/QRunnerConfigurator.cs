using System.Diagnostics;
using QuickAcid.Reporting;

namespace QuickAcid.Bolts;


public class QRunnerConfigurator
{
    private readonly QAcidScript<Acid> script;
    private readonly int? seed;

    public QRunnerConfigurator(QAcidScript<Acid> script, int? seed)
    {
        this.script = script;
        this.seed = seed;
    }

    [StackTraceHidden]
    public QRunner With(RunCount runCount) => new(script, runCount, seed);
    [StackTraceHidden]
    public QRunner WithOneRun() => With(new(1));
    [StackTraceHidden]
    public Report WithOneRunAndOneExecution() => With(new(1)).AndOneExecutionPerRun();
}

public record RunCount(int NumberOfRuns);
public record ExecutionCount(int NumberOfExecutions);


