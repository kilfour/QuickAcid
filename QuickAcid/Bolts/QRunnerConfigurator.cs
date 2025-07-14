using System.Diagnostics;
using QuickAcid.Reporting;

namespace QuickAcid.Bolts;


public class QRunnerConfigurator
{
    private readonly QAcidScript<Acid> script;

    public QRunnerConfigurator(QAcidScript<Acid> script)
    {
        this.script = script;
    }

    [StackTraceHidden]
    public QRunner With(RunCount runCount) => new(script, runCount);
    [StackTraceHidden]
    public QRunner WithOneRun() => With(new(1));
    [StackTraceHidden]
    public Report WithOneRunAndOneExecution() => With(new(1)).AndOneExecutionPerRun();
}

public record RunCount(int NumberOfRuns);
public record ExecutionCount(int NumberOfExecutions);


