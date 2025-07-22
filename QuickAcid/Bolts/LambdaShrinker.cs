using QuickAcid.Bolts.ShrinkStrats;

namespace QuickAcid.Bolts;

public class LambdaShrinker<T> : IShrinker<T>
{
    private readonly Func<T, IEnumerable<T>> shrinker;

    public LambdaShrinker(Func<T, IEnumerable<T>> shrinker) => this.shrinker = shrinker;

    public IEnumerable<T> Shrink(T value) => shrinker(value);
}