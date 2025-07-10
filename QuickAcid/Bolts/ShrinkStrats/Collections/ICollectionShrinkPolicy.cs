namespace QuickAcid.Bolts.ShrinkStrats.Collections;

public interface ICollectionShrinkPolicy
{
    IEnumerable<ICollectionShrinkStrategy> GetStrategies();
}
