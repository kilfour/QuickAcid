using QuickAcid.Fluent;

namespace QuickAcid.Tests.Fluent.Spec;

public class ExpectTests
{
    [Fact]
    public void Expect_returns_true_nothing_happens()
    {
        var report =
            SystemSpecs.Define()
                .Expect("should be true").Ensure(() => true)
                .DumpItInAcid()
                .AndCheckForGold(1, 1);
        Assert.Null(report);
    }

    [Fact]
    public void Expect_returns_false_gets_reported()
    {
        var report =
            SystemSpecs.Define()
                .Expect("should be true")
                    .Ensure(() => false)
                .DumpItInAcid()
                .AndCheckForGold(1, 1);
        Assert.NotNull(report);
        var entry = report.GetSpecEntry();
        Assert.NotNull(entry);
        Assert.Equal("should be true", entry.Key);
    }

    [Fact]
    public void Expect_with_perform_returns_true_nothing_happens()
    {
        var flag = false;
        var report =
            SystemSpecs.Define()
                .As("flag it").Now(() => flag = true)
                .Expect("should be true").Ensure(() => flag == true)
                .DumpItInAcid()
                .AndCheckForGold(1, 1);
        Assert.Null(report);
        Assert.True(flag);
    }

    [Fact]
    public void Expect_with_perform_returns_false_gets_reported()
    {
        var flag = false;
        var report =
            SystemSpecs.Define()
                .As("flag it").Now(() => flag = true)
                .Expect("should be true").Ensure(() => flag == false)
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
    public void Expect_can_access_the_context()
    {
        var report =
            SystemSpecs.Define()
                .Tracked(Keys.TheAnswer, _ => 42)
                .Expect("should be true")
                    .Ensure(ctx => ctx.Get(Keys.TheAnswer) == 42)
                .DumpItInAcid()
                .AndCheckForGold(1, 1);
        Assert.Null(report);
    }

    [Fact]
    public void Expect_can_access_the_typed_context_content()
    {
        var report =
            SystemSpecs.Define()
                .Tracked(Keys.TheAnswer, _ => 42)
                .Expect("should be true")
                    .UseThe(Keys.TheAnswer)
                    .Ensure(a => a == 42)
                .DumpItInAcid()
                .AndCheckForGold(1, 1);
        Assert.Null(report);
    }

    [Fact]
    public void Expect_can_access_the_typed_context_content_with_correct_tea()
    {
        var report =
            SystemSpecs.Define()
                .Tracked(Keys.TheAnswer, _ => 42)
                .Do("stuff", () => { })
                .Expect("should be true")
                    .UseThe(Keys.TheAnswer)
                    .Ensure(a => a == 42)
                .DumpItInAcid()
                .AndCheckForGold(1, 1);
        Assert.Null(report);
    }

    //// ==== ⛔ INTENTIONALLY NOT COMPILABLE TEST ====
    // This test is *meant* to fail compilation if uncommented.
    // It's proof the fluent API prevents invalid usage.
    // 
    // [Fact]
    // public void Expect_without_use_the_and_typed_context_does_not_compile()
    // {
    //     var report =
    //         SystemSpecs.Define()
    //             .TrackedInput(Keys.TheAnswer, _ => 42)
    //             .Expect("should be true")
    //                 .Ensure(a => a == 42)
    //             .DumpItInAcid()
    //             .AndCheckForGold(1, 1);
    //     Assert.Null(report);
    // }
    // ✅ This is a feature.
    //// ==============================================
}