using QuickAcid.Bolts.ShrinkStrats;

namespace QuickAcid.Bolts.Nuts;

public static class Shrink<T>
{
    public static QAcidRunner<Acid> LikeThis(IShrinker<T> shrinker)
    {
        return
            state =>
                {
                    state.RegisterShrinker(shrinker);
                    return QAcidResult.AcidOnly(state);
                };
    }

    public static QAcidRunner<Acid> LikeThis(Func<T, IEnumerable<T>> shrinker)
    {
        return LikeThis(new LambdaShrinker<T>(shrinker));
    }
}
