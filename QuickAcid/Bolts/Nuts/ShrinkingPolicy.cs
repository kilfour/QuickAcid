using QuickAcid.Bolts.ShrinkStrats.Collections;
using QuickAcid.Bolts.ShrinkStrats.Objects;

namespace QuickAcid.Bolts.Nuts;

public static class ShrinkingPolicy
{
    public static QAcidScript<Acid> ForCollections(params ICollectionShrinkStrategy[] strategies) =>
        state =>
            {
                state.GetCollectionStrategies = () => strategies;
                return QAcidResult.AcidOnly(state);
            };

    public static QAcidScript<Acid> ForObjects(params IObjectShrinkStrategy[] strategies) =>
        state =>
            {
                state.GetObjectStrategies = () => strategies;
                return QAcidResult.AcidOnly(state);
            };
}