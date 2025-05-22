namespace QuickAcid.Bolts.ShrinkStrats;

public interface IShrinker<T>
{
    IEnumerable<T> Shrink(T value);
}

public interface ICShrinker<T>
{
    IEnumerable<T> Shrink(T value);
}