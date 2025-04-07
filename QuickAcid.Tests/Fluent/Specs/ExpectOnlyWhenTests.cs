using QuickAcid.Fluent;

namespace QuickAcid.Tests.Fluent.Expect;

public class ExpectOnlyWhenTests
{
    [Fact]
    public void Expect_should_not_run_if_predicate_is_false()
    {
        var report =
            SystemSpecs.Define()
                .Expect("should be true")
                    .OnlyWhen(() => false)
                    .Ensure(() => true)
                .DumpItInAcid()
                .AndCheckForGold(1, 1);
        Assert.Null(report);
    }

    [Fact]
    public void Expect_should_run_if_predicate_is_true()
    {
        var report =
            SystemSpecs.Define()
                .Expect("should be true")
                    .OnlyWhen(() => true)
                    .Ensure(() => false)
                .DumpItInAcid()
                .AndCheckForGold(1, 1);
        Assert.NotNull(report);
        var entry = report.GetSpecEntry();
        Assert.NotNull(entry);
        Assert.Equal("should be true", entry.Key);
    }
}