using System.Diagnostics;
using QuickAcid.Reporting;

namespace QuickAcid.Bolts;

public record QRunnerConfig(bool Verbose);

public class QRunnerConfigurator
{
    private readonly QAcidScript<Acid> script;
    private readonly int? seed;
    private QRunnerConfig config;

    public QRunnerConfigurator(QAcidScript<Acid> script, int? seed)
    {
        this.script = script;
        this.seed = seed;
        config = new QRunnerConfig(Verbose: false);
    }

    [StackTraceHidden]
    public QRunnerConfigurator Options(Func<QRunnerConfig, QRunnerConfig> modify)
    {
        config = modify(config);
        return this;
    }

    [StackTraceHidden]
    public QRunner With(RunCount runCount) => new(script, config, runCount, seed);
    [StackTraceHidden]
    public QRunner WithOneRun() => With(new(1));
    [StackTraceHidden]
    public Report WithOneRunAndOneExecution() => With(new(1)).AndOneExecutionPerRun();
}

public record RunCount(int NumberOfRuns);
public record ExecutionCount(int NumberOfExecutions);


