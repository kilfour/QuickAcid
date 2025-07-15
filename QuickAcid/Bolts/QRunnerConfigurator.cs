using System.Diagnostics;
using QuickAcid.Bolts.ShrinkStrats;
using QuickAcid.Reporting;
using QuickPulse;
using QuickPulse.Bolts;

namespace QuickAcid.Bolts;

public record QRunnerConfig
{
    public required bool AddShrinkInfoToReport { get; init; }
    public Flow<ShrinkTrace>? ShrinkTraceFlow { get; init; }
    public required string? ReportTo { get; init; }
    public required string? Vault { get; init; }
    public required bool Verbose { get; init; }

    public static QRunnerConfig Default()
    {
        return
            new QRunnerConfig()
            {
                AddShrinkInfoToReport = false,
                ShrinkTraceFlow =
                    from input in Pulse.Start<ShrinkTrace>()
                    from _ in Pulse.Trace($"  {input}")
                    select input,
                ReportTo = null,
                Vault = null,
                Verbose = false,
            };
    }
}

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
            config = config with { ReportTo = testName, Vault = testName };
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
    public Report WithOneRunAndOneExecution() => With(new(1)).AndOneExecutionPerRun();
}

public record RunCount(int NumberOfRuns);
public record ExecutionCount(int NumberOfExecutions);


