namespace QuickAcid.TestsDeposition.Docs.Logging;

public class SpecReportingTest
{
    [Fact]
    public void Count()
    {
        var counter = 0;
        var script =
            from act in "act".Act(() => counter++)
            from s1 in "Spec One".Spec(() => true)
            from s2 in "Spec Two".SpecIf(() => counter > 2, () => counter < 4)
            from s3 in "Spec Three".Spec(() => true)
            select Acid.Test;
        var report = new QState(script).Observe(10);

        Assert.Equal("Spec One", report.PassedSpecCount[0].Label);
        Assert.Equal(4, report.PassedSpecCount[0].Count);

        Assert.Equal("Spec Three", report.PassedSpecCount[1].Label);
        Assert.Equal(3, report.PassedSpecCount[1].Count);

        Assert.Equal("Spec Two", report.PassedSpecCount[2].Label);
        Assert.Equal(1, report.PassedSpecCount[2].Count);
    }
}