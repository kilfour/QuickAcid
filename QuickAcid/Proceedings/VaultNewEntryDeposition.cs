namespace QuickAcid.Proceedings;

public record VaultNewEntryDeposition
{
    public VaultNewEntryDeposition(int seed)
    {
        Seed = seed;
    }
    public int Seed { get; }
}
