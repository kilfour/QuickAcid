namespace QuickAcid.Bolts.ShrinkStrats;

public class DefaultCollectionShrinkingPolicy : ICollectionShrinkPolicy
{
    public IEnumerable<ICollectionShrinkStrategy> GetStrategies() =>
        [ new RemoveOneByOneStrategy()
        , new ShrinkEachElementStrategy()
        ];
}
