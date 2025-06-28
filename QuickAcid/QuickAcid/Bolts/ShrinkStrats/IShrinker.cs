namespace QuickAcid.Bolts.ShrinkStrats;

public interface IShrinker<T>
{
    IEnumerable<T> Shrink(T value);
}