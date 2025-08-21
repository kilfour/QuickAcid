namespace QuickAcid.Proceedings;

public record VaultDeposition
{
    public List<VaultRetryDeposition> VaultRetryDepositions { get; } = [];
    public VaultDeposition AddPassedSpecDeposition(VaultRetryDeposition vaultRetryDeposition)
    {
        VaultRetryDepositions.Add(vaultRetryDeposition);
        return this;
    }
}
