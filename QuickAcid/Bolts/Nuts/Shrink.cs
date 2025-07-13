using System.Linq.Expressions;
using QuickAcid.Bolts.ShrinkStrats;

namespace QuickAcid.Bolts.Nuts;

public static class Shrink<T>
{
    public static QAcidScript<Acid> LikeThis(IShrinker<T> shrinker) =>
        state =>
            {
                state.RegisterShrinker(shrinker);
                return QAcidResult.AcidOnly(state);
            };


    public static QAcidScript<Acid> LikeThis(Func<T, IEnumerable<T>> shrinker)
        => LikeThis(new LambdaShrinker<T>(shrinker));

    public static QAcidScript<Acid> For<TProp>(Expression<Func<T, TProp>> expr, IShrinker<TProp> shrinker) =>
        state =>
            {
                state.RegisterPropertyShrinker(expr, shrinker);
                return QAcidResult.AcidOnly(state);
            };

    public static QAcidScript<Acid> For<TProp>(Expression<Func<T, TProp>> expr, Func<TProp, IEnumerable<TProp>> shrinker)
        => For(expr, new LambdaShrinker<TProp>(shrinker));

}
