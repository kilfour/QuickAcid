using QuickAcid;
using QuickAcid.Bolts.ShrinkStrats;
using QuickPulse;
using QuickPulse.Bolts;
using QuickPulse.Show;

namespace QuickAcid.RunningThings;

public record QRunnerConfig
{
    public required bool AddShrinkInfoToReport { get; init; }
    public Flow<ShrinkTrace>? ShrinkTraceFlow { get; init; }

    public required string? ReportTo { get; init; }
    public required string? Vault { get; init; }
    public required bool ReplayMode { get; init; }

    //----------------------------------------------------------------
    // Options passed to QAcidState
    // --
    public required bool Verbose { get; init; }
    public required bool ShrinkingActions { get; init; }
    //----------------------------------------------------------------

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
                ReplayMode = false,
                ShrinkingActions = false
            };
    }

    public static readonly Flow<ShrinkTrace> Raw =
        from input in Pulse.Start<ShrinkTrace>()
        from _ in Pulse.Trace($"  {input}")
        select input;

    private static readonly Flow<ShrinkTrace> DefaultFormat =
        from input in Pulse.Start<ShrinkTrace>()
        from _ in Pulse.TraceIf(input.Intent == ShrinkIntent.Irrelevant,
            () => $"  {input.Key} = {Introduce.This(input.Original!, false)}, ExecId = {input.ExecutionId}, Intent = {input.Intent} (Cause: {Introduce.This(input.Result!, false)}), Strategy = {input.Strategy} ")
        from __ in Pulse.TraceIf(input.Intent != ShrinkIntent.Irrelevant,
            () => $"  {input.Key} = {Introduce.This(input.Original!, false)}, ExecId = {input.ExecutionId}, Intent = {input.Intent}, Strategy = {input.Strategy}")
        select input;
}


