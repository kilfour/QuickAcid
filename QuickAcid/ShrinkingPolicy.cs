using QuickAcid.Bolts;
using QuickAcid.Bolts.ShrinkStrats.Objects;
using QuickAcid.Shrinking.Collections;

namespace QuickAcid;

public static class ShrinkingPolicy
{
    public static QAcidScript<Acid> ForCollections(params ICollectionShrinkStrategy[] strategies) =>
        state =>
            {
                state.ShrinkingRegistry.GetCollectionStrategies = () => strategies;
                return QAcidResult.AcidOnly(state);
            };

    public static QAcidScript<Acid> ForObjects(params IObjectShrinkStrategy[] strategies) =>
        state =>
            {
                state.ShrinkingRegistry.GetObjectStrategies = () => strategies;
                return QAcidResult.AcidOnly(state);
            };
}