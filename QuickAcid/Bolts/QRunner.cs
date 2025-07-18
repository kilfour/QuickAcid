using System.Diagnostics;
using QuickAcid.Reporting;
using QuickMGenerate;
using QuickPulse;
using QuickPulse.Arteries;

namespace QuickAcid.Bolts;

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

    [StackTraceHidden]
    public Report And(ExecutionCount executionCount)
    {
        Report report = null!;
        if (config.ReplayMode)
        {
            if (Replay())
                return ReportIt(new Report());
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
            report = state.Run(executionCount.NumberOfExecutions);
            state.GetPassedSpecCount(passedSpecCount);
            if (report.IsFailed)
            {
                AddToVault(state.Seed, executionCount.NumberOfExecutions);
                throw new FalsifiableException(ReportIt(report));
            }
        }
        return ReportIt(report);
    }

    private Report ReportIt(Report report)
    {
        AddPassedSpecsToReport(report);
        AddShrinkTracesToReport(report);
        AddVaultMessagesToReport(report);
        WriteReport(report);
        return report;
    }

    private void AddShrinkTracesToReport(Report report)
    {
        if (config.AddShrinkInfoToReport)
        {
            report.AddEntry(new ReportInfoEntry(" Shrink Info"));
            Signal.From(config.ShrinkTraceFlow!)
                .SetArtery(new ReportArtery(report))
                .Pulse(report.ShrinkTraces);
            report.AddEntry(new ReportInfoEntry(" " + new string('─', 50)));
        }
    }

    private void WriteReport(Report report)
    {
        if (config.ReportTo != null)
        {
            var filename = Path.Combine(".quickacid", "reports", $"{config.ReportTo}.qr");
            Signal.Tracing<string>().SetArtery(new WriteDataToFile(filename).ClearFile())
                   .Pulse(report.Entries.Select(a => a.ToString()!));
        }
    }
    private bool Replay()
    {
        if (new SeedVault(config.Vault!).AllSeeds.Count == 0)
            return false;
        CheckTheVault();
        return true;
    }

    private void CheckTheVault()
    {
        vaultMessages.Add("Checking the Vault");
        var vault = new SeedVault(config.Vault!);
        foreach (var vaultEntry in vault.AllSeeds)
        {
            var state = new QAcidState(script, vaultEntry.Seed);
            var report = state.Run(vaultEntry.ExecutionsPerRun);
            if (report.IsSuccess)
            {
                vault.Remove(vaultEntry);
                vaultMessages.Add($"Seed {vaultEntry.Seed} now passes => removed.");
            }
            else
            {
                vaultMessages.Add($"Seed {vaultEntry.Seed}: still fails.");
            }
        }
    }

    private void AddToVault(int seed, int numberOfExecutions)
    {
        if (config.Vault == null) return;
        var vault = new SeedVault(config.Vault!);
        vault.Add(new(seed, numberOfExecutions));
        vaultMessages.Add($"Seed {seed}: added to vault.");
    }

    private void AddPassedSpecsToReport(Report report)
    {
        if (passedSpecCount.Count > 0)
        {
            report.AddEntry(new ReportInfoEntry($" Passed Specs"));
            passedSpecCount.ForEach(kv => report.AddEntry(new ReportInfoEntry($"  - {kv.Key}: {kv.Value}x")));
            report.AddEntry(new ReportInfoEntry(" " + new string('─', 50)));
        }
    }

    private void AddVaultMessagesToReport(Report report)
    {
        if (vaultMessages.Count > 0)
        {
            report.AddEntry(new ReportInfoEntry($" Vault Info"));
            vaultMessages.ForEach(a => report.AddEntry(new ReportInfoEntry($"   {a}")));
            report.AddEntry(new ReportInfoEntry(" " + new string('─', 50)));
        }
    }

    [StackTraceHidden]
    public Report AndOneExecutionPerRun() => And(new ExecutionCount(1));
}