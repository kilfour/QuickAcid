using System.Diagnostics;
using QuickAcid.Bolts;
using QuickAcid.Proceedings;
using QuickAcid.Proceedings.ClerksOffice;
using QuickFuzzr;
using QuickPulse;
using QuickPulse.Arteries;

namespace QuickAcid.RunningThings;

public class QRunner
{
    private readonly QAcidScript<Acid> script;
    private readonly QRunnerConfig config;
    private readonly RunCount runCount;
    private readonly int? seed;

    public Dictionary<string, int> passedSpecCount { get; } = [];

    public QRunner(QAcidScript<Acid> script, QRunnerConfig config, RunCount runCount, int? seed)
    {
        this.script = script;
        this.config = config;
        this.runCount = runCount;
        this.seed = seed;
    }

    // -------------------------------------------------------------------------
    // This method starts the actual test
    // --
    [StackTraceHidden]
    public CaseFile And(ExecutionCount executionCount)
    {
        var vault = config.Vault == null ? new EmptyVault() : new SeedVault(config.Vault!, script);
        CaseFile caseFile = null!;
        if (config.ReplayMode)
        {
            if (vault.Replay())
                return FileIt(CaseFile.Empty());
        }
        if (config.Vault != null)
        {
            vault.Check();
        }
        for (int i = 0; i < runCount.NumberOfRuns; i++)
        {
            var state = seed.HasValue ? new QAcidState(script, seed.Value) : new QAcidState(script);
            if (config.Verbose)
                state.Verbose = true;
            if (config.ShrinkingActions)
                state.ShrinkingActions = true;
            caseFile = state.Run(executionCount.NumberOfExecutions);
            state.GetPassedSpecCount(passedSpecCount);
            if (caseFile.HasVerdict())
            {
                vault.AddSeed(state.Seed, executionCount.NumberOfExecutions);
                //vaultMessages.Add($"Seed {seed}: added to vault.");
                throw new FalsifiableException(FileIt(caseFile));
            }
        }
        return FileIt(caseFile);
    }

    private CaseFile FileIt(CaseFile caseFile)
    {
        AddPassedSpecsToCaseFile(caseFile);
        //AddVaultMessagesToCaseFile(caseFile);
        WriteCaseFile(caseFile);
        return caseFile;
    }

    private void WriteCaseFile(CaseFile caseFile)
    {
        if (config.FileAs != null)
        {
            var filenameCf = Path.Combine(".quickacid", "archive", $"{config.FileAs}.qr");
            Signal.Tracing<string>().SetArtery(new WriteDataToFile(filenameCf).ClearFile())
                   .Pulse(TheClerk.Transcribes(caseFile));
        }
    }

    private void AddPassedSpecsToCaseFile(CaseFile caseFile)
    {
        passedSpecCount.ForEach(kv =>
            caseFile.AddPassedSpecDeposition(new PassedSpecDeposition(kv.Key, kv.Value)));
    }

    [StackTraceHidden]
    public CaseFile AndOneExecutionPerRun() => And(new ExecutionCount(1));
}