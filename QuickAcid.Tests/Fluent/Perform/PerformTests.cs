using QuickAcid.Fluent;
using QuickAcid.Reporting;

namespace QuickAcid.Tests.Fluent.Perform;
public class PerformTests
{
    [Fact]
    public void Perform_should_do_its_action()
    {
        var flag = false;
        var report =
            SystemSpecs.Define()
                .Perform("flag it", () => flag = true)
                .DumpItInAcid()
                .AndCheckForGold(1, 1);
        Assert.Null(report);
        Assert.True(flag);
    }

    [Fact]
    public void Perform_should_do_its_action_even_if_theres_more_of_them()
    {
        var flag = "";
        var report =
            SystemSpecs.Define()
                .Perform("flag it once", () => flag += "a")
                .Perform("flag it again", () => flag += "b")
                .DumpItInAcid()
                .AndCheckForGold(1, 1);
        Assert.Null(report);
        Assert.Equal("ab", flag);
    }

    [Fact]
    public void Perform_that_throws_should_report_failure()
    {
        var report =
            SystemSpecs.Define()
                .Perform("throws", () => throw new Exception("Boom"))
                .DumpItInAcid()
                .AndCheckForGold(1, 1);
        Assert.NotNull(report);

        var entry = report.FirstOrDefault<ReportActEntry>();
        Assert.NotNull(entry);
        Assert.Equal("throws", entry.Key);
        Assert.NotNull(entry.Exception);
        Assert.Equal("Boom", entry.Exception.Message);
    }
}