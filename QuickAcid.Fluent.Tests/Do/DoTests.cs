using QuickAcid.Fluent;
using QuickAcid.Reporting;

namespace QuickAcid.Tests.Fluent.Do;

public class DoTests
{
    public static class Keys
    {
        public static QKey<bool> TheBool =>
            QKey<bool>.New("TheBool");
    }

    [Fact]
    public void Do_should_do_its_action()
    {
        var flag = false;
        var report =
            SystemSpecs.Define()
                .Do("flag it", () => flag = true)
                .DumpItInAcid()
                .AndCheckForGold(1, 1);
        Assert.Null(report);
        Assert.True(flag);
    }

    [Fact]
    public void Do_should_do_its_action_even_if_theres_more_of_them()
    {
        var flag = "";
        var report =
            SystemSpecs.Define()
                .Do("flag it once", () => flag += "a")
                .Do("flag it again", () => flag += "b")
                .DumpItInAcid()
                .AndCheckForGold(1, 1);
        Assert.Null(report);
        Assert.Equal("ab", flag);
    }

    [Fact]
    public void Do_that_throws_should_report_failure()
    {
        var report =
            SystemSpecs.Define()
                .Do("throws", () => throw new Exception("Boom"))
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
    public void Do_can_use_context()
    {
        var flag = false;
        var report =
            SystemSpecs.Define()
                .Tracked(Keys.TheBool, () => true)
                .Do("flag it", ctx => flag = ctx.Get(Keys.TheBool))
                .DumpItInAcid()
                .AndCheckForGold(1, 1);
        Assert.Null(report);
        Assert.True(flag);
    }
}