using QuickAcid.Fluent;
using QuickAcid.Reporting;


namespace QuickAcid.Tests.Fluent.Perform;

public class ContextTests
{
    public class Container
    {
        public int ItsOnlyAModel { get; set; }
        public override string ToString()
        {
            return ItsOnlyAModel.ToString();
        }
    }
    public static class Keys
    {
        public static QKey<Container> Universe => QKey<Container>.New("universe");
        public static QKey<int> TheAnswer => QKey<int>.New("theAnswer");
    }

    [Fact]
    public void TrackedInput_can_be_accessed_through_context()
    {
        var theAnswer = 0;
        var report =
            SystemSpecs.Define()
                .TrackedInput(Keys.Universe, () => new Container() { ItsOnlyAModel = 42 })
                .As("Answering the Question").UseThe(Keys.Universe)
                    .Now(universe => theAnswer = universe.ItsOnlyAModel)
                .DumpItInAcid()
                .AndCheckForGold(1, 1);
        Assert.Null(report);
        Assert.Equal(42, theAnswer);
    }

    [Fact]
    public void Context_access_to_non_existing_value_throws()
    {
        var theAnswer = 0;
        var report =
            SystemSpecs.Define()
                .Do("arthur", ctx => () =>
                    theAnswer = ctx.GetItAtYourOwnRisk<Container>("not there").ItsOnlyAModel)
                .DumpItInAcid()
                .AndCheckForGold(1, 1);
        Assert.NotNull(report);
        var entry = report.FirstOrDefault<ReportActEntry>();
        Assert.NotNull(entry);
        Assert.NotNull(entry.Exception);
        Assert.IsType<ThisNotesOnYou>(entry.Exception);
        Assert.Equal("The value for key 'not there' was not present in memory, ... and neither was the key itself.", entry.Exception.Message);
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

