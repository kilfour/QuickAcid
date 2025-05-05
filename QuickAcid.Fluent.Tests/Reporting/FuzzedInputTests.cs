using QuickAcid.Reporting;
using QuickMGenerate;

namespace QuickAcid.Tests.Reporting;

public class FuzzedInputTests
{
    [Fact]
    public void FuzzedInput_irrelevant_should_always_be_reported_twice_when_using_the_touchstone()
    {
        var report =
            SystemSpecs
                .Define()
                .Fuzzed("fuzzy", MGen.Constant(42))
                .Assert("Fail", _ => false)
                .DumpItInAcid()
                .KeepOneEyeOnTheTouchStone()
                .AndCheckForGold(1, 1);
        var entries = report.OfType<ReportInputEntry>();
        Assert.Equal(2, entries.Count());
    }
}