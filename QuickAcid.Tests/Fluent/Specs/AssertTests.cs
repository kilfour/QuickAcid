using QuickAcid.Fluent;


namespace QuickAcid.Tests.Fluent.Spec;

public class AssertTests
{
    [Fact]
    public void Assert_returns_true_nothing_happens()
    {
        var report =
            SystemSpecs.DefineN()
                .Assert("should be true", () => true)
                .DumpItInAcid()
                .AndCheckForGold(1, 1);
        Assert.Null(report);
    }

    [Fact]
    public void Assert_returns_false_gets_reported()
    {
        var report =
            SystemSpecs.DefineN()
                .Assert("should be true", () => false)
                .DumpItInAcid()
                .AndCheckForGold(1, 1);
        Assert.NotNull(report);
        var entry = report.GetSpecEntry();
        Assert.NotNull(entry);
        Assert.Equal("should be true", entry.Key);
    }

    [Fact]
    public void Assert_with_perform_returns_true_nothing_happens()
    {
        var flag = false;
        var report =
            SystemSpecs.Define()
                .As("flag it").Now(() => flag = true)
                .Assert("should be true", () => flag == true)
                .DumpItInAcid()
                .AndCheckForGold(1, 1);
        Assert.Null(report);
        Assert.True(flag);
    }

    [Fact]
    public void Assert_with_perform_returns_false_gets_reported()
    {
        var flag = false;
        var report =
            SystemSpecs.Define()
                .As("flag it").Now(() => flag = true)
                .Assert("should be true", () => flag == false)
                .DumpItInAcid()
                .AndCheckForGold(1, 1);
        Assert.NotNull(report);
        var entry = report.GetSpecEntry();
        Assert.NotNull(entry);
        Assert.Equal("should be true", entry.Key);
    }
}