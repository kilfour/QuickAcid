using System.Linq.Expressions;
using QuickAcid.Bolts;
using QuickAcid.Shrinking.Custom;

namespace QuickAcid;

public static class Shrink<T>
{
    public static QAcidScript<Acid> LikeThis(IShrinker<T> shrinker) =>
        state =>
            {
                state.ShrinkingRegistry.Register(shrinker);
                return Vessel.AcidOnly(state);
            };

    public static QAcidScript<Acid> LikeThis(Func<T, IEnumerable<T>> shrinker)
        => LikeThis(new LambdaShrinker<T>(shrinker));

    public static QAcidScript<Acid> None()
        => LikeThis(new LambdaShrinker<T>(_ => []));

    public static QAcidScript<Acid> For<TProp>(Expression<Func<T, TProp>> expr, IShrinker<TProp> shrinker) =>
        state =>
            {
                state.ShrinkingRegistry.RegisterPropertyShrinker(expr, shrinker);
                return Vessel.AcidOnly(state);
            };

    public static QAcidScript<Acid> For<TProp>(Expression<Func<T, TProp>> expr, Func<TProp, IEnumerable<TProp>> shrinker)
        => For(expr, new LambdaShrinker<TProp>(shrinker));

    public static QAcidScript<Acid> None<TProp>(Expression<Func<T, TProp>> expr)
        => For(expr, new LambdaShrinker<TProp>(_ => []));

}
