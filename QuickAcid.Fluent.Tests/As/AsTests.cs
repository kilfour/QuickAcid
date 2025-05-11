using QuickAcid.Fluent;
using QuickAcid.Reporting;

namespace QuickAcid.Tests.Fluent.As;

public class AsTests
{
    public static class Keys
    {
        public static QKey<bool> TheBool =>
            QKey<bool>.New("TheBool");
    }

    [Fact]
    public void As_should_do_its_action()
    {
        var flag = false;
        var report =
            SystemSpecs.Define()
                .As("flag it").Now(() => flag = true)
                .DumpItInAcid()
                .AndCheckForGold(1, 1);
        Assert.Null(report);
        Assert.True(flag);
    }

    [Fact]
    public void As_should_do_its_action_even_if_theres_more_of_them()
    {
        var flag = "";
        var report =
            SystemSpecs.Define()
                .As("flag it once").Now(() => flag += "a")
                .As("flag it again").Now(() => flag += "b")
                .DumpItInAcid()
                .AndCheckForGold(1, 1);
        Assert.Null(report);
        Assert.Equal("ab", flag);
    }

    [Fact]
    public void As_that_throws_should_report_failure()
    {
        var report =
            SystemSpecs.Define()
                .As("throws").Now(() => throw new Exception("Boom"))
                .DumpItInAcid()
                .AndCheckForGold(1, 1);
        Assert.NotNull(report);

        var entry = report.FirstOrDefault<ReportExecutionEntry>();
        Assert.NotNull(entry);
        Assert.Equal("throws", entry.Key);
        Assert.NotNull(report.Exception);
        Assert.Equal("Boom", report.Exception.Message);
    }

    [Fact]
    public void As_can_use_context()
    {
        var flag = false;
        var report =
            SystemSpecs.Define()
                .Tracked(Keys.TheBool, () => true)
                .As("flag it").Now(ctx => flag = ctx.Get(Keys.TheBool))
                .DumpItInAcid()
                .AndCheckForGold(1, 1);
        Assert.Null(report);
        Assert.True(flag);
    }
}