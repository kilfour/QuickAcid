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
        var flag = false;
        var report =
            SystemSpecs.Define()
                .Do("flag it", () => flag = true)
                .Do("flag it", () => flag = true)
                .DumpItInAcid()
                .AndCheckForGold(1, 1);
        Assert.Null(report);
        Assert.True(flag);
    }
}