namespace QuickAcid.TheyCanFade;

public class MemoryLens
{
    public Func<object, object> Get { get; init; }
    public Func<object, object, object> Set { get; init; }
    public MemoryLens(Func<object, object> get, Func<object, object, object> set) { Get = get; Set = set; }
}