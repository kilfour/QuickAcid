using QuickAcid.Tests._Tools.ThePress;
using QuickFuzzr;
using StringExtensionCombinators;

namespace QuickAcid.Tests.Spikes.StoringStuff;

public class StashTests
{
    [Fact]
    public void First_try()
    {

        var script =
            from stash in Script.StashFor<int>()
            from cnt in "cnt".Input(Fuzzr.Counter(0))
            from create in "create a thing".Act(() => cnt)
            from add in Script.Execute(() => stash.Add(cnt))
            from blowup in "boom".Spec(() => !stash.Where(a => a == 5).Any() && stash.Count != 2)
            select Acid.Test;

        var article = TheJournalist.Exposes(() =>
            QState.Run(script)
            .Options(a => a with { FileAs = "StashTests" })
            .WithOneRun()
            .And(10.ExecutionsPerRun()));

        // var actEntry1 = article.Execution(1).Action(1).Read();
        // Assert.NotNull(actEntry1);
        // Assert.Equal("create a thing", actEntry1.Label);

        // var actEntry2 = article.Execution(2).Action(1).Read();
        // Assert.NotNull(actEntry2);
        // Assert.Equal("next state", actEntry2.Label);

    }
}