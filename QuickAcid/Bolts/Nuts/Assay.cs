namespace QuickAcid.Bolts.Nuts;

public static partial class QAcid
{
    public static QAcidRunner<Acid> Assay(this string key, Func<bool> condition)
    {
        return state =>
        {
            state.AddAssay(key, condition);
            return QAcidResult.AcidOnly(state);
        };
    }
}