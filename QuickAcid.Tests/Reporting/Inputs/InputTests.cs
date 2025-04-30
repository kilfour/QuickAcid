using QuickAcid.Reporting;
using QuickMGenerate;
using QuickAcid.Bolts.Nuts;
using QuickAcid.Bolts;


namespace QuickAcid.Tests.Reporting.Inputs;

public class InputTests
{
    [Fact]
    public void Null_shows_up_in_report_as_null()
    {
        var run =
            from input in "input".Shrinkable(MGen.Constant<string?>(null))
            from foo in "spec".Spec(() => input != null)
            select Acid.Test;

        var report = run.ReportIfFailed();
        Assert.NotNull(report);

        var entry = report.FirstOrDefault<ReportInputEntry>();
        Assert.NotNull(entry);
        Assert.Equal("input", entry.Key);
        Assert.Equal("<null>", entry.Value);
    }

    [Fact]
    public void Empty_String_shows_up_in_report_as_empty()
    {
        var run =
            from input in "input".Shrinkable(MGen.Constant(""))
            from foo in "spec".Spec(() => !string.IsNullOrEmpty(input))
            select Acid.Test;

        var report = run.ReportIfFailed();
        Assert.NotNull(report);

        var entry = report.FirstOrDefault<ReportInputEntry>();
        Assert.NotNull(entry);
        Assert.Equal("input", entry.Key);
        Assert.Equal("\"\"", entry.Value);
    }

    [Fact]
    public void String_shows_up_in_report_with_quotes()
    {
        var run =
            from input in "input".Shrinkable(MGen.Constant(" "))
            from foo in "spec".Spec(() => string.IsNullOrEmpty(input))
            select Acid.Test;

        var report = run.ReportIfFailed();
        Assert.NotNull(report);

        var entry = report.FirstOrDefault<ReportInputEntry>();
        Assert.NotNull(entry);
        Assert.Equal("input", entry.Key);
        Assert.Equal("\" \"", entry.Value);
    }
}