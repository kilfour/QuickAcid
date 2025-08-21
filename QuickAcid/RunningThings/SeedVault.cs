using QuickAcid;
using QuickAcid.Bolts;
using QuickPulse.Instruments;

namespace QuickAcid.RunningThings;

public record VaultEntry(int Seed, int ExecutionsPerRun);


public class EmptyVault
{
    public virtual void Check() { }
    public virtual bool Replay() { return false; }
    public virtual void AddSeed(int seed, int numberOfExecutions) { }
}

public class SeedVault : EmptyVault
{
    private readonly string path;
    private readonly HashSet<VaultEntry> entries;
    private readonly QAcidScript<Acid> script;

    public SeedVault(string name, QAcidScript<Acid> script)
    {
        var root = SolutionLocator.FindSolutionRoot();
        root ??= AppContext.BaseDirectory;
        path = Path.Combine(root!, ".quickacid", "contested", $"{name}.qv");
        entries = GetVaultEntries();
        this.script = script;
    }

    public override void Check()
    {
        foreach (var vaultEntry in entries)
        {
            var state = new QAcidState(script, vaultEntry.Seed);
            var caseFile = state.Run(vaultEntry.ExecutionsPerRun);
            if (caseFile.HasVerdict())
            {
                // vaultMessages.Add($"Seed {vaultEntry.Seed}: still fails.");
            }
            else
            {
                Remove(vaultEntry);
                // vaultMessages.Add($"Seed {vaultEntry.Seed} now passes => removed.");
            }
        }
    }

    public override bool Replay()
    {
        if (entries.Count == 0)
            return false;
        Check();
        return true;
    }

    public override void AddSeed(int seed, int numberOfExecutions)
    {
        Add(new(seed, numberOfExecutions));
    }

    private void Add(VaultEntry vaultEntry)
    {
        if (entries.Add(vaultEntry))
            Save();
    }
    private HashSet<VaultEntry> GetVaultEntries()
    {
        return File.Exists(path) ? ReadEntriesFromFile() : [];
    }

    private HashSet<VaultEntry> ReadEntriesFromFile()
    {
        return File.ReadAllLines(path)
            .Select(ParseVaultEntry)
            .Where(entry => entry != null)
            .ToHashSet()!;
    }

    private static VaultEntry ParseVaultEntry(string line)
    {
        var parts = line.Split(',');
        return parts.Length == 2 &&
            int.TryParse(parts[0], out var seed) &&
            int.TryParse(parts[1], out var executions)
            ? new VaultEntry(seed, executions)
            : null!;
    }

    private void Remove(VaultEntry vaultEntry)
    {
        if (entries.Remove(vaultEntry))
            SaveOrDelete();
    }

    private void Save()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllLines(path, entries.Select(a => $"{a.Seed},{a.ExecutionsPerRun}"));
    }

    private void SaveOrDelete()
    {
        if (entries.Count == 0)
        {
            if (File.Exists(path))
                File.Delete(path);
        }
        else
        {
            Save();
        }
    }
}