using QuickAcid.Fluent;
using QuickAcid.Reporting;

namespace QuickAcid.Tests.Fluent.Do;

public class MultipleDoTests
{
    public static class Keys
    {
        public static QKey<bool> TheBool =>
            QKey<bool>.New("TheBool");
    }

    [Fact]
    public void Do_should_do_its_actionS_in_one_execution()
    {
        var flag1 = false;
        var flag2 = false;
        var report =
            SystemSpecs.Define()
                .Do("flag it once", () => flag1 = true)
                .Do("flag it twice", () => flag2 = true)
                .DumpItInAcid()
                .AndCheckForGold(1, 1);
        Assert.Null(report);
        Assert.True(flag1);
        Assert.True(flag2);
    }

    [Fact]
    public void Do_should_do_report_actionS_in_one_execution()
    {

        var report =
            SystemSpecs.Define()
                .Do("foo", () => { })
                .Do("bar", () => { })
                .Assert("fail", _ => false)
                .DumpItInAcid()
                .AndCheckForGold(1, 1);
        Assert.NotNull(report);
        var entry = report.FirstOrDefault<ReportExecutionEntry>();
        Assert.NotNull(entry);
        Assert.Equal("", entry.Key);
    }
}