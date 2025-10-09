namespace QuickAcid.Tests.Spikes.Reducers;

public class FirstShot
{
    [Fact]
    public void CheckTowardsZero() => Assert.Equal([4, 3, 2, 1, 0], TowardsZero(5));

    private IEnumerable<int> TowardsZero(int input)
    {
        var local = input;
        while (local != 0)
        {
            if (local > 0)
                yield return --local;
            else
                yield return ++local;
        }
    }
}