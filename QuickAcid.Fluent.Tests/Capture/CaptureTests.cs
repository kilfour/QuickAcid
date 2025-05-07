using QuickAcid.Fluent;

namespace QuickAcid.Tests.Fluent.TrackedInput;

public class CaptureTests
{
    public static class Keys
    {
        public static readonly QKey<Container> Container =
            QKey<Container>.New("container");
        public static QKey<int> Property =>
            QKey<int>.New("Property");
    }

    public class Container
    {
        public int ItsOnlyAModel { get; set; }
        public override string ToString()
        {
            return ItsOnlyAModel.ToString();
        }
    }

    [Fact]
    public void Capture_acts_as_a_closure()
    {
        var theContainerProperty = 0;
        var theCapturedProperty = 0;
        var report =
            SystemSpecs
                .Define()
                .Tracked(Keys.Container, () => new Container() { ItsOnlyAModel = 1 })
                .Capture(Keys.Property, ctx => ctx.Get(Keys.Container).ItsOnlyAModel)
                .Do("increment", ctx => { ctx.Get(Keys.Container).ItsOnlyAModel++; })
                .Do("report", ctx =>
                {
                    theContainerProperty = ctx.Get(Keys.Container).ItsOnlyAModel;
                    theCapturedProperty = ctx.Get(Keys.Property);
                })
                .DumpItInAcid()
                .AndCheckForGold(1, 1);
        Assert.Null(report);
        Assert.Equal(2, theContainerProperty);
        Assert.Equal(1, theCapturedProperty);
    }

    [Fact]
    public void Capture_checking_before_and_after()
    {
        var theCapturedPropertyBefore = 0;
        var theCapturedPropertyAfter = 0;
        var report =
            SystemSpecs
                .Define()
                .Tracked(Keys.Container, () => new Container() { ItsOnlyAModel = 1 })
                .Capture("before", ctx => ctx.Get(Keys.Container).ItsOnlyAModel)
                .Do("increment", ctx => { ctx.Get(Keys.Container).ItsOnlyAModel++; })
                .Capture("after", ctx => ctx.Get(Keys.Container).ItsOnlyAModel)
                .Do("report", ctx =>
                {
                    theCapturedPropertyBefore = ctx.GetItAtYourOwnRisk<int>("before");
                    theCapturedPropertyAfter = ctx.GetItAtYourOwnRisk<int>("after");
                })
                .DumpItInAcid()
                .AndCheckForGold(1, 1);
        Assert.Null(report);
        Assert.Equal(1, theCapturedPropertyBefore);
        Assert.Equal(2, theCapturedPropertyAfter);
    }

    [Fact]
    public void Capture_never_shows_up_in_report()
    {
        var report =
            SystemSpecs
                .Define()
                .Tracked(Keys.Container, () => new Container() { ItsOnlyAModel = 1 })
                .Capture(Keys.Property, ctx => ctx.Get(Keys.Container).ItsOnlyAModel)
                .Do("increment", ctx => { throw new Exception(); })
                .DumpItInAcid()
                .AndCheckForGold(1, 1);
        Assert.NotNull(report);
        Assert.DoesNotContain("Property", report.ToString());
    }

    [Fact]
    public void Capture_never_shows_up_in_report_with_touch()
    {
        var report =
            SystemSpecs
                .Define()
                .Tracked(Keys.Container, () => new Container() { ItsOnlyAModel = 1 })
                .Capture(Keys.Property, ctx => ctx.Get(Keys.Container).ItsOnlyAModel)
                .Do("increment", ctx => { throw new Exception(); })
                .DumpItInAcid()
                .KeepOneEyeOnTheTouchStone()
                .AndCheckForGold(1, 1);
        Assert.NotNull(report);
        Assert.DoesNotContain("Property", report.ToString());
    }
}