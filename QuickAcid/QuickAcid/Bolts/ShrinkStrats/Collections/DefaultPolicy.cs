namespace QuickAcid.Bolts.ShrinkStrats.Collections;

public class DefaultCollectionShrinkingPolicy : ICollectionShrinkPolicy
{
    public IEnumerable<ICollectionShrinkStrategy> GetStrategies() =>
        [ new RemoveOneByOneStrategy()
        , new ShrinkEachElementStrategy()
        ];
}
