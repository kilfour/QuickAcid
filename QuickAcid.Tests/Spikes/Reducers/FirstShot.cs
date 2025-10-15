using QuickAcid.Tests._Tools.ThePress;
using QuickFuzzr;
using StringExtensionCombinators;

namespace QuickAcid.Tests.Spikes.Reducers;

public class FirstShot
{
    [Fact]
    public void HowTo()
    {
        var script =
            from i in "i".Input(Fuzz.Constant(5), a => a.ReduceWith(TowardsZero))
            from a in "a".Act(() => i)
            from s in "s".Spec(() => a < 3)
            select Acid.Test;
        var article = TheJournalist.Exposes(() => QState.Run(script).WithOneRunAndOneExecution());
        var inputDeposition = article.Execution(1).Input(1).Read();
        Assert.NotNull(inputDeposition);
        Assert.Equal("i", inputDeposition.Label);
        Assert.Equal("3", inputDeposition.Value);
    }

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