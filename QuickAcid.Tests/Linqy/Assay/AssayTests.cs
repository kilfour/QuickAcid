using QuickAcid.Bolts;
using QuickAcid.Bolts.Nuts;
using QuickAcid.Bolts.Nuts.QuickMGenerateExtensions;
using QuickAcid.Reporting;
using QuickMGenerate;

namespace QuickAcid.Tests.Linqy.Assay;

public class AssayTests
{
    [Fact]
    public void Spike()
    {
        var run =
            from observer in "observer".AlwaysReported(() => new HashSet<int>())
            from roll in "roll".Act(() => MGen.Int(1, 3).Generate())
            from _a1 in "record".Act(() => observer.Add(roll))
            from as1 in "gens 3".Assay(() => observer.Contains(3))
            select Acid.Test;

        var report = run.ReportIfFailed(1, 20);

        Assert.NotNull(report);
        var entry = report.FirstOrDefault<ReportTitleSectionEntry>();
        Assert.NotNull(entry);
        Assert.Contains("The Assayer disagrees : gens 3.", entry.ToString());
    }
}