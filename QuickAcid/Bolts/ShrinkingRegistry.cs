using System.Linq.Expressions;
using System.Reflection;
using QuickAcid.Bolts.ShrinkStrats;
using QuickAcid.Bolts.ShrinkStrats.Collections;
using QuickAcid.Bolts.ShrinkStrats.Objects;
using QuickFuzzr.UnderTheHood;

namespace QuickAcid.Bolts;

public class ShrinkingRegistry
{
    private readonly Dictionary<Type, object> shrinkers = [];

    public void RegisterShrinker<T>(IShrinker<T> shrinker)
    {
        shrinkers[typeof(T)] = shrinker;
    }

    public IShrinker<T>? TryGetShrinker<T>()
    {
        return shrinkers.TryGetValue(typeof(T), out var shrinker)
            ? shrinker as IShrinker<T>
            : null;
    }

    private readonly Dictionary<(Type, PropertyInfo), IShrinkerBox> propertyShrinkers = [];

    public void RegisterPropertyShrinker<T, TProp>(Expression<Func<T, TProp>> expr, IShrinker<TProp> shrinker)
    {
        var info = expr.AsPropertyInfo();
        propertyShrinkers[(typeof(T), info)] = new ShrinkerBox<TProp>(shrinker);
    }

    public IShrinkerBox? TryGetPropertyShrinker<T>(PropertyInfo info)
    {
        return propertyShrinkers.TryGetValue((typeof(T), info), out var shrinker)
            ? shrinker
            : null;
    }

    public Func<IEnumerable<ICollectionShrinkStrategy>> GetCollectionStrategies =
        () => [new RemoveOneByOneStrategy(), new GreedyShrinkEachElementStrategy(), new ShrinkEachElementStrategy()];

    public Func<IEnumerable<IObjectShrinkStrategy>> GetObjectStrategies =
        () => [new ObjectShrinkStrategy()];
}