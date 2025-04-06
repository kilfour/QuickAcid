using QuickAcid.Fluent;
using QuickAcid.Reporting;


namespace QuickAcid.Tests.Fluent.Perform;

public class TrackedInputTests
{
    public static class Keys
    {
        public static readonly QKey<Container> Container =
            QKey<Container>.New("container");
        public static QKey<Container> Universe =>
            QKey<Container>.New("universe");
        public static QKey<int> TheAnswer =>
            QKey<int>.New("theAnswer");
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
    public void TrackedInput_should_exist_in_per_action_report()
    {
        var report =
            SystemSpecs
                .Define()
                .TrackedInput(Keys.Container, () => new Container() { ItsOnlyAModel = 1 })
                .As("throw").Now(() => throw new Exception())
                .Do("throw", _ => throw new Exception())
                .DumpItInAcid()
                .AndCheckForGold(1, 1);
        Assert.NotNull(report);

        var entry = report.FirstOrDefault<QAcidReportTrackedInputEntry>();
        Assert.NotNull(entry);
        Assert.Equal("container", entry.Key);
        Assert.Equal("container (tracked) : 1", entry.ToString());
    }

    [Fact]
    public void TrackedInput_can_use_context_when_registering()
    {
        var container = new Container();
        var report =
            SystemSpecs.Define()
                .TrackedInput(Keys.TheAnswer, () => 42)
                .TrackedInput(Keys.Universe, ctx => { container.ItsOnlyAModel = ctx.Get(Keys.TheAnswer); return container; })
                .DumpItInAcid()
                .AndCheckForGold(1, 1);
        Assert.Null(report);
        Assert.Equal(42, container.ItsOnlyAModel);
    }
}