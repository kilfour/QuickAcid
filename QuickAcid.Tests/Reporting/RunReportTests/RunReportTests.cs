using QuickAcid.Reporting;
using QuickAcid.Bolts.TheyCanFade;

namespace QuickAcid.Tests.Reporting.RunReportTests;

public class RunReportTests
{
    private readonly int IrrelevantNumberForReporting = 666;
    private Memory EmptyMemory =>
        new(() => IrrelevantNumberForReporting);

    [Fact]
    public void RunReport_can_be_converted_to_stringlist()
    {
        var report = new RunReport("It's only a model");
        Assert.NotNull(report.AsStringList());
    }

    [Fact]
    public void RunReport_contains_its_given_title()
    {
        var report = new RunReport("It's only a model");
        var result = string.Join("", report.AsStringList());
        Assert.Contains("It's only a model", result);
    }

    [Fact]
    public void RunReport_last_string_indicates_start_of_run()
    {
        var report = new RunReport("It's only a model");
        var result = report.AsStringList().Last();
        Assert.NotNull(result);
        Assert.Equal("RUN START :", result);
    }
}