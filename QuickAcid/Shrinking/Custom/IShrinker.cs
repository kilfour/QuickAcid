namespace QuickAcid.Shrinking.Custom;

public interface IShrinkerBox
{
    IEnumerable<object> Shrink(object value);
}

public class ShrinkerBox<T> : IShrinkerBox
{
    private readonly IShrinker<T> inner;

    public ShrinkerBox(IShrinker<T> inner)
    {
        this.inner = inner;
    }

    public IEnumerable<object> Shrink(object value)
    {
        return inner.Shrink((T)value).Cast<object>();
    }
}

public interface IShrinker<T>
{
    IEnumerable<T> Shrink(T value);
}