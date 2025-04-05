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

    [Fact]
    public void TrackedInput_should_exist_in_per_action_report()
    {
        var report =
            SystemSpecs.Define()
                .TrackedInput("container", () => new Container() { ItsOnlyAModel = 1 })
                .Perform("throw", () => throw new Exception())
                .DumpItInAcid()
                .CheckForGold(1, 1);
        Assert.NotNull(report);

        var entry = report.FirstOrDefault<QAcidReportTrackedInputEntry>();
        Assert.NotNull(entry);
        Assert.Equal("container", entry.Key);
        Assert.Equal("container (tracked) : 1", entry.ToString());
    }
}