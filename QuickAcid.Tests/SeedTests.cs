using QuickAcid.Bolts;
using QuickAcid.Reporting;
using QuickMGenerate;
using QuickPulse.Arteries;

namespace QuickAcid.Tests;

public class SeedTests
{
    [Fact]
    public void Seed_Shows_Up_In_Report()
    {
        var script =
            from act in "act".Act(() => { })
            from spec in "spec".Spec(() => false)
            select Acid.Test;

        var report = new QState(script).Observe(10);
        var entry = report.Single<ReportTitleSectionEntry>();

        Assert.Equal(4, entry.Title.Count);

        var line = entry.Title[3];
        Assert.StartsWith("Seed: ", line);
        var seedPart = line.Substring("Seed: ".Length);
        Assert.True(int.TryParse(seedPart, out _), $"Expected an int after 'Seed: ', but got: {seedPart}");
    }

    [Fact]
    public void Seed_Allows_Reruns()
    {
        var collector = new TheCollector<int>();
        var script =
            from input in "input".Input(MGen.Int())
            from act in "act".Act(() => collector.Flow(input))
            from spec in "spec".Spec(() => collector.TheExhibit.Count != 2)
            select Acid.Test;
        var report = new QState(script, 42).Observe(10);
        var entry = report.Single<ReportCollapsedExecutionEntry>();
        Assert.Equal("act", entry.Key);
        Assert.Equal([67, 14, 14, 67, 67, 14, 67, 67, 14, 14], collector.TheExhibit);
    }
}