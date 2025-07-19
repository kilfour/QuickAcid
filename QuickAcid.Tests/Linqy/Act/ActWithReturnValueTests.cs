using QuickAcid.Reporting;

namespace QuickAcid.Tests.Linqy.Act;

public class ActWithReturnValueTests
{
    [Fact]
    public void Simple()
    {
        var script = from foo in "foo".Act(() => 42) select Acid.Test;
        var report = new QState(script).AlwaysReport().ObserveOnce();
        var entry = report.FirstOrDefault<ReportExecutionEntry>();
        Assert.NotNull(entry);
        Assert.Equal("foo", entry.Key);
    }

    [Fact]
    public void Stringify()
    {
        // var script = from foo in "foo".Act(() => 42, a => a.ToString()) select Acid.Test;
        // var report = new QState(script).AlwaysReport().ObserveOnce();
        // var entry = report.FirstOrDefault<ReportExecutionEntry>();
        // Assert.NotNull(entry);
        // Assert.Equal("foo => 42", entry.Key);
    }
}
