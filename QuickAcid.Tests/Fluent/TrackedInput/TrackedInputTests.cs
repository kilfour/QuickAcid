using QuickAcid.Fluent;
using QuickAcid.Reporting;


namespace QuickAcid.Tests.Fluent.Perform;

public class TrackedInputTests
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
        public static readonly QKey<Container> Container =
            QKey<Container>.New("container");
    }

    [Fact]
    public void TrackedInput_should_exist_in_per_action_report()
    {
        var report =
            SystemSpecs
                .Define()
                .TrackedInput(Keys.Container, () => new Container() { ItsOnlyAModel = 1 })
                .Perform("throw", () => throw new Exception())
                .DumpItInAcid()
                .AndCheckForGold(1, 1);
        Assert.NotNull(report);

        var entry = report.FirstOrDefault<QAcidReportTrackedInputEntry>();
        Assert.NotNull(entry);
        Assert.Equal("container", entry.Key);
        Assert.Equal("container (tracked) : 1", entry.ToString());
    }
}