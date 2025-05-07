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

    public static class Keys
    {
        public static readonly QKey<int> TheAnswer =
            QKey<int>.New("TheAnswer");
    }

    [Fact]
    public void Expect_OnlyWhen_can_access_the_context()
    {
        var report =
            SystemSpecs.Define()
                .Tracked(Keys.TheAnswer, _ => 42)
                    .Expect("should be true")
                    .OnlyWhen(ctx => ctx.Get(Keys.TheAnswer) == 42)
                    .Ensure(a => false)
                .DumpItInAcid()
                .AndCheckForGold(1, 1);
        Assert.NotNull(report);
    }

    [Fact]
    public void Expect_OnlyWhen_can_access_the_typed_context_content()
    {
        var report =
            SystemSpecs.Define()
                .Tracked(Keys.TheAnswer, _ => 42)
                    .Expect("should be true", Keys.TheAnswer)
                    .OnlyWhen(a => a == 42)
                    .Ensure(a => false)
                .DumpItInAcid()
                .AndCheckForGold(1, 1);
        Assert.NotNull(report);
    }

    [Fact]
    public void Expect_OnlyWhen_can_access_the_typed_context_content_with_use()
    {
        var report =
            SystemSpecs.Define()
                .Tracked(Keys.TheAnswer, _ => 42)
                .Expect("should be true")
                    .UseThe(Keys.TheAnswer)
                    .OnlyWhen(a => a == 42)
                    .Ensure(a => false)
                .DumpItInAcid()
                .AndCheckForGold(1, 1);
        Assert.NotNull(report);
    }
}