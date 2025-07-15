using QuickPulse.Instruments;

namespace QuickAcid.Bolts;

public record VaultEntry(int Seed, int ExecutionsPerRun);

public sealed class SeedVault
{
    private readonly string path;
    private readonly HashSet<VaultEntry> entries;

    public SeedVault(string name)
    {
        var root = SolutionLocator.FindSolutionRoot();
        root ??= AppContext.BaseDirectory;
        path = Path.Combine(root!, ".quickacid", "failures", $"{name}.qv");
        entries = File.Exists(path)
            ? File.ReadAllLines(path)
                .Select(line =>
                {
                    var parts = line.Split(',');
                    return parts.Length == 2 &&
                        int.TryParse(parts[0], out var seed) &&
                        int.TryParse(parts[1], out var executions)
                        ? new VaultEntry(seed, executions)
                        : null;
                })
                .Where(entry => entry != null)
                .ToHashSet()!
            : new HashSet<VaultEntry>();
    }

    public IReadOnlyCollection<VaultEntry> AllSeeds => entries;

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