using QuickAcid.Fluent;

namespace QuickAcid.Tests.Fluent.Spec;

public class SpecTests
{
    [Fact]
    public void Spec_returns_true_nothing_happens()
    {
        var report =
            SystemSpecs.Define()
                .Spec("should be true").Assert(() => true)
                .DumpItInAcid()
                .AndCheckForGold(1, 1);
        Assert.Null(report);
    }

    [Fact]
    public void Spec_returns_false_gets_reported()
    {
        var report =
            SystemSpecs.Define()
                .Spec("should be true")
                    .Assert(() => false)
                .DumpItInAcid()
                .AndCheckForGold(1, 1);
        Assert.NotNull(report);
        var entry = report.GetSpecEntry();
        Assert.NotNull(entry);
        Assert.Equal("should be true", entry.Key);
    }

    [Fact]
    public void Spec_with_perform_returns_true_nothing_happens()
    {
        var flag = false;
        var report =
            SystemSpecs.Define()
                .Perform("flag it", () => flag = true)
                .Spec("should be true").Assert(() => flag == true)
                .DumpItInAcid()
                .AndCheckForGold(1, 1);
        Assert.Null(report);
        Assert.True(flag);
    }

    [Fact]
    public void Spec_with_perform_returns_false_gets_reported()
    {
        var flag = false;
        var report =
            SystemSpecs.Define()
                .Perform("flag it", () => flag = true)
                .Spec("should be true").Assert(() => flag == false)
                .DumpItInAcid()
                .AndCheckForGold(1, 1);
        Assert.NotNull(report);
        var entry = report.GetSpecEntry();
        Assert.NotNull(entry);
        Assert.Equal("should be true", entry.Key);
    }
}