using System.Diagnostics;
using QuickAcid.Proceedings;

namespace QuickAcid.RunningThings;

public class QRunnerConfigurator
{
    private readonly QAcidScript<Acid> script;
    private readonly int? seed;
    private QRunnerConfig config;

    public QRunnerConfigurator(string? testName, QAcidScript<Acid> script, int? seed)
    {
        this.script = script;
        this.seed = seed;
        config = QRunnerConfig.Default();
        if (testName != null)
        {
            config = config with { FileAs = testName, Vault = testName };
        }
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
    public CaseFile WithOneRunAndOneExecution() => With(new(1)).AndOneExecutionPerRun();
}

public record RunCount(int NumberOfRuns);
public record ExecutionCount(int NumberOfExecutions);


