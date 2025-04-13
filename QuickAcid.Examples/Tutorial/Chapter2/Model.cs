namespace QuickAcid.Examples.Tutorial.Chapter2;

public class LoadedDie
{
    private readonly Random _random = new();
    public int Roll() => _random.Next(1, 7);
}