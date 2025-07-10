using QuickAcid.Bolts.ShrinkStrats.Collections;

namespace QuickAcid.Bolts.Nuts;

public static class ShrinkingPolicy
{
    public static QAcidScript<Acid> ForCollections(params ICollectionShrinkStrategy[] strategies) =>
        state =>
            {
                state.GetCollectionStrategies = () => strategies;
                return QAcidResult.AcidOnly(state);
            };
}