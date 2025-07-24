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
    private readonly List<string> vaultMessages = [];

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
        CaseFile caseFile = null!;
        if (config.ReplayMode)
        {
            if (Replay())
                return FileIt(CaseFile.Empty()); // need to get the verdicts ?
        }
        if (config.Vault != null)
        {
            CheckTheVault();
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
                AddToVault(state.Seed, executionCount.NumberOfExecutions);
                throw new FalsifiableException(FileIt(caseFile));
            }
        }
        return FileIt(caseFile);
    }

    private CaseFile FileIt(CaseFile caseFile)
    {
        AddPassedSpecsToCaseFile(caseFile);
        AddVaultMessagesToCaseFile(caseFile);
        WriteCaseFile(caseFile);
        return caseFile;
    }

    private void WriteCaseFile(CaseFile caseFile)
    {
        if (config.ReportTo != null)
        {
            var filenameCf = Path.Combine(".quickacid", "caseFiles", $"{config.ReportTo}.qr");
            Signal.Tracing<string>().SetArtery(new WriteDataToFile(filenameCf).ClearFile())
                   .Pulse(TheClerk.Transcribes(caseFile));
        }
    }
    private bool Replay()
    {
        if (new SeedVault(config.Vault!, script).AllSeeds.Count == 0)
            return false;
        CheckTheVault();
        return true;
    }

    private void CheckTheVault()
    {
        vaultMessages.Add("Checking the Vault");
        var vault = new SeedVault(config.Vault!, script);
        foreach (var vaultEntry in vault.AllSeeds)
        {
            var state = new QAcidState(script, vaultEntry.Seed);
            var caseFile = state.Run(vaultEntry.ExecutionsPerRun);
            if (caseFile.HasVerdict())
            {
                vaultMessages.Add($"Seed {vaultEntry.Seed}: still fails.");
            }
            else
            {
                vault.Remove(vaultEntry);
                vaultMessages.Add($"Seed {vaultEntry.Seed} now passes => removed.");
            }
        }
    }

    private void AddToVault(int seed, int numberOfExecutions)
    {
        if (config.Vault == null) return;
        var vault = new SeedVault(config.Vault!, script);
        vault.Add(new(seed, numberOfExecutions));
        vaultMessages.Add($"Seed {seed}: added to vault.");
    }

    private void AddPassedSpecsToCaseFile(CaseFile caseFile)
    {
        if (passedSpecCount.Count > 0)
        {
            var extraDeposition = new ExtraDeposition("Passed Specs");
            passedSpecCount.ForEach(kv => extraDeposition.AddMessage($"- {kv.Key}: {kv.Value}x"));
            caseFile.AddExtraDeposition(extraDeposition);
        }
    }

    private void AddVaultMessagesToCaseFile(CaseFile caseFile)
    {
        if (vaultMessages.Count > 0)
        {
            var extraDeposition = new ExtraDeposition("Vault Info");
            vaultMessages.ForEach(a => extraDeposition.AddMessage(a));
            caseFile.AddExtraDeposition(extraDeposition);
        }
    }

    [StackTraceHidden]
    public CaseFile AndOneExecutionPerRun() => And(new ExecutionCount(1));
}