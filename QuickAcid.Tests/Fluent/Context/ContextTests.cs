using System.Security.Cryptography;
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

    [Fact]
    public void TrackedInput_can_be_accessed_through_context()
    {
        var theAnswer = 0;
        var report =
            SystemSpecs.Define()
                .TrackedInput("universe", () => new Container() { ItsOnlyAModel = 42 })
                .Perform("arthur", ctx => () =>
                    theAnswer = ctx.Get<Container>("universe").ItsOnlyAModel)
                .DumpItInAcid()
                .CheckForGold(1, 1);
        Assert.Null(report);
        Assert.Equal(42, theAnswer);
    }

    [Fact]
    public void Context_access_to_non_existing_value_throws()
    {
        var theAnswer = 0;
        var report =
            SystemSpecs.Define()
                .Perform("arthur", ctx => () =>
                    theAnswer = ctx.Get<Container>("not there").ItsOnlyAModel)
                .DumpItInAcid()
                .CheckForGold(1, 1);
        Assert.NotNull(report);
        var entry = report.FirstOrDefault<ReportActEntry>();
        Assert.NotNull(entry);
        Assert.NotNull(entry.Exception);
        Assert.IsType<ThisNotesOnYou>(entry.Exception);
        Assert.Equal("The value for key 'not there' was not present in memory, ... and neither was the key itself.", entry.Exception.Message);
    }
}

