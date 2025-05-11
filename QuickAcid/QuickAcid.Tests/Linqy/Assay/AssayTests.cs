using QuickAcid.Bolts;
using QuickAcid.Bolts.Nuts;
using QuickAcid.Reporting;
using QuickMGenerate;

namespace QuickAcid.Tests.Linqy.Assay;

public class AssayTests
{
    [Fact]
    public void Assay_something_did_not_happen()
    {
        var script =
            from observer in "observer".Tracked(() => new HashSet<int>())
            from roll in "roll".Act(() => MGen.Int(1, 3).Generate())
            from _a1 in "record".Act(() => observer.Add(roll))
            from as1 in "gens 3".Assay(() => observer.Contains(3))
            select Acid.Test;

        var report = new QState(script).Observe(20);

        Assert.NotNull(report);
        var entry = report.FirstOrDefault<ReportTitleSectionEntry>();
        Assert.NotNull(entry);
        Assert.Contains("The Assayer disagrees: gens 3.", entry.ToString());
    }

    [Fact]
    public void Assay_multiple_did_not_happen()
    {
        var script =
            from observer in "observer".Tracked(() => new HashSet<int>())
            from roll in "roll".Act(() => MGen.Int(1, 3).Generate())
            from _a1 in "record".Act(() => observer.Add(roll))
            from as1 in "combined".Assay(("gens 3", () => observer.Contains(3)), ("gens 4", () => observer.Contains(4)))
            select Acid.Test;

        var report = new QState(script).Observe(20);

        Assert.NotNull(report);
        var entry = report.FirstOrDefault<ReportTitleSectionEntry>();
        Assert.NotNull(entry);
        Assert.Contains("The Assayer disagrees: gens 3, gens 4.", entry.ToString());
    }
}