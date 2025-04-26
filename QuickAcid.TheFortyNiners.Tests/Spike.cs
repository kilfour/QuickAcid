using QuickAcid.Bolts;
using QuickAcid.Bolts.Nuts;

namespace QuickAcid.TheFortyNiners.Tests;

public class Spike
{
    public class Question { public virtual int Answer() => 42; }

    [Fact]
    public void Something()
    {
        var run =
            from question in "question".Stashed(() => new Question())
            select Acid.Test;
        var report = run.ReportIfFailed(1, 1);
    }
}
