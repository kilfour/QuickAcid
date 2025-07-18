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
    public required bool ReplayMode { get; init; }

    public static QRunnerConfig Default()
    {
        return
            new QRunnerConfig()
            {
                AddShrinkInfoToReport = false,
                ShrinkTraceFlow = DefaultFormat,
                ReportTo = null,
                Vault = null,
                Verbose = false,
                ReplayMode = false
            };
    }

    public static readonly Flow<ShrinkTrace> Raw =
        from input in Pulse.Start<ShrinkTrace>()
        from _ in Pulse.Trace($"  {input}")
        select input;

    private static readonly Flow<ShrinkTrace> DefaultFormat =
        from input in Pulse.Start<ShrinkTrace>()
        from _ in Pulse.TraceIf(input.Intent == ShrinkIntent.Irrelevant,
            $"  {input.Key} = {QuickAcidStringify.Default()(input.Original!)}, ExecId = {input.ExecutionId}, Intent = {input.Intent} (Cause: {QuickAcidStringify.Default()(input.Result!)}), Strategy = {input.Strategy} ")
        from __ in Pulse.TraceIf(input.Intent != ShrinkIntent.Irrelevant,
            $"  {input.Key} = {QuickAcidStringify.Default()(input.Original!)}, ExecId = {input.ExecutionId}, Intent = {input.Intent}, Strategy = {input.Strategy}")
        select input;
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


