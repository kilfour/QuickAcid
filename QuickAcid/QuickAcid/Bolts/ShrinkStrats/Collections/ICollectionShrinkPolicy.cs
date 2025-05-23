namespace QuickAcid.Bolts.ShrinkStrats;

public interface ICollectionShrinkPolicy
{
    IEnumerable<ICollectionShrinkStrategy> GetStrategies();
}
