namespace QuickAcid.Proceedings;

public record VaultRetryDeposition
{
    public VaultRetryDeposition(int seed, bool passed)
    {
        Seed = seed;
        Passed = passed;
    }
    public bool Passed { get; }
    public int Seed { get; }
}
