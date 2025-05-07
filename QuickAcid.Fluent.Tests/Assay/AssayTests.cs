using QuickAcid.Fluent;
using QuickAcid.Reporting;
using QuickMGenerate;
using QuickMGenerate.UnderTheHood;

namespace QuickAcid.Tests.Fluent.Assay;

public class AssayTests
{
    [Fact]
    public void Assay_works_but_could_do_with_more_tests()
    {
        var report =
            SystemSpecs.Define()
                .Tracked("die", () => MGen.Int(1, 3))
                .Tracked("observer", () => new HashSet<int>())
                .Do("roll",
                    c =>
                        c.GetItAtYourOwnRisk<HashSet<int>>("observer")
                             .Add(c.GetItAtYourOwnRisk<Generator<int>>("die").Generate()))
                .Assay("gens 3", c => c.GetItAtYourOwnRisk<HashSet<int>>("observer").Contains(3))
                .DumpItInAcid()
                .AndCheckForGold(1, 20);

        Assert.NotNull(report);
        var entry = report.FirstOrDefault<ReportTitleSectionEntry>();
        Assert.NotNull(entry);
        Assert.Contains("The Assayer disagrees: gens 3.", entry.ToString());
    }
}