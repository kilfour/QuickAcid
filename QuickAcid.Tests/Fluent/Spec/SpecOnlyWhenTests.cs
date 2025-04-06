using QuickAcid.Fluent;

namespace QuickAcid.Tests.Fluent.Spec;

public class SpecOnlyWhenTests
{
    [Fact]
    public void Spec_should_not_run_if_predicate_is_false()
    {
        var report =
            SystemSpecs.Define()
                .Spec("should be true")
                    .OnlyWhen(() => false)
                    .Assert(() => true)
                .DumpItInAcid()
                .AndCheckForGold(1, 1);
        Assert.Null(report);
    }

    [Fact]
    public void Spec_should_run_if_predicate_is_true()
    {
        var report =
            SystemSpecs.Define()
                .Spec("should be true")
                    .OnlyWhen(() => true)
                    .Assert(() => false)
                .DumpItInAcid()
                .AndCheckForGold(1, 1);
        Assert.NotNull(report);
        var entry = report.GetSpecEntry();
        Assert.NotNull(entry);
        Assert.Equal("should be true", entry.Key);
    }
}