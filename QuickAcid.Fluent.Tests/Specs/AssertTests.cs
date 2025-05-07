using QuickAcid.Fluent;

namespace QuickAcid.Tests.Fluent.Spec;

public class AssertTests
{
    [Fact]
    public void Assert_returns_true_nothing_happens()
    {
        var report =
            SystemSpecs.Define()
                .Assert("should be true", _ => true)
                .DumpItInAcid()
                .AndCheckForGold(1, 1);
        Assert.Null(report);
    }

    [Fact]
    public void Assert_returns_false_gets_reported()
    {
        var report =
            SystemSpecs.Define()
                .Assert("should be true", _ => false)
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
                .As("flag it").Now(_ => flag = true)
                .Assert("should be true", _ => flag == true)
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
                .As("flag it").Now(_ => flag = true)
                .Assert("should be true", _ => flag == false)
                .DumpItInAcid()
                .AndCheckForGold(1, 1);
        Assert.NotNull(report);
        var entry = report.GetSpecEntry();
        Assert.NotNull(entry);
        Assert.Equal("should be true", entry.Key);
    }

    public static class K
    {
        public static readonly QKey<int> TheAnswer =
            QKey<int>.New("TheAnswer");
    }

    [Fact]
    public void Assert_can_access_the_context()
    {
        var report =
            SystemSpecs.Define()
                .Tracked(K.TheAnswer, _ => 42)
                .Assert("should be true", ctx => ctx.Get(K.TheAnswer) == 42)
                .DumpItInAcid()
                .AndCheckForGold(1, 1);
        Assert.Null(report);
    }
}