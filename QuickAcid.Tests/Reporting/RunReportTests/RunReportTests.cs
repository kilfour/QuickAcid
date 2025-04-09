using QuickAcid.Reporting;

namespace QuickAcid.Tests.Reporting.RunReportTests;

public class RunReportTests : QAcidLoggingFixture
{
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