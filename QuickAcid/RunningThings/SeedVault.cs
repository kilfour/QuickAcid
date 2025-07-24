using QuickAcid;
using QuickPulse.Instruments;

namespace QuickAcid.RunningThings;

public record VaultEntry(int Seed, int ExecutionsPerRun);

public class SeedVault
{
    private readonly string path;
    private readonly HashSet<VaultEntry> entries;
    private readonly QAcidScript<Acid> script;

    public IReadOnlyCollection<VaultEntry> AllSeeds => entries;

    public SeedVault(string name, QAcidScript<Acid> script)
    {
        var root = SolutionLocator.FindSolutionRoot();
        root ??= AppContext.BaseDirectory;
        path = Path.Combine(root!, ".quickacid", "contested", $"{name}.qv");
        entries = GetVaultEntries();
        this.script = script;
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

    public void AddToVault(int seed, int numberOfExecutions)
    {
        Add(new(seed, numberOfExecutions));
    }

    public bool Contains(VaultEntry vaultEntry) => entries.Contains(vaultEntry);

    public void Add(VaultEntry vaultEntry)
    {
        if (entries.Add(vaultEntry))
            Save();
    }

    public void Remove(VaultEntry vaultEntry)
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