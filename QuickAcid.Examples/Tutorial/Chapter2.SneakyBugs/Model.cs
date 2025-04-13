namespace QuickAcid.Examples.Tutorial.Chapter2.SneakyBugs;

public class LoadedDie
{
    private readonly Random _random = new();
    // this is the correct implementation
    // see about for more info
    public int Roll() => _random.Next(1, 7);
}