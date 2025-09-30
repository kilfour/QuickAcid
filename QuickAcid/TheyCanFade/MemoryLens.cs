namespace QuickAcid.TheyCanFade;

public class MemoryLens(
    Func<object, object> get,
    Func<object, object, object> set)
{
    public Func<object, object> Get { get; init; } = get;
    public Func<object, object, object> Set { get; init; } = set;
}