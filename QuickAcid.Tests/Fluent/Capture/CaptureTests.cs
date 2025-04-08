using QuickAcid.Fluent;



namespace QuickAcid.Tests.Fluent.AlwaysReportedInput;

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
                .AlwaysReported(Keys.Container, () => new Container() { ItsOnlyAModel = 1 })
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
}