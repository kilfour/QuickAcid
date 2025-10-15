using QuickAcid.Tests._Tools.ThePress;

namespace QuickAcid.TestsDeposition.Docs.Combinators.Choose;


public class ChooseIfTests
{
    [Fact]

    public void Usage()
    {
        var script =
            from observer in Script.Stashed(() => new HashSet<int>())
            from _ in Script.ChooseIf(
                (() => true, "One".Act(() => observer.Add(1))),
                (() => false, "Two".Act(() => observer.Add(2))),
                (() => true, "Three".Act(() => observer.Add(3))))
            from as1 in "Action Assert".Assay(
                ("One Seen", () => observer.Contains(1)),
                ("Two Not Seen", () => !observer.Contains(2)),
                ("Three Seen", () => observer.Contains(3)))
            select Acid.Test;

        var caseFile = QState.Run("temp", script)
            .WithOneRun()
            .And(20.ExecutionsPerRun());
        var article = TheJournalist.Unearths(caseFile);
        Assert.False(article.VerdictReached());
    }
}

