using System.Diagnostics;
using QuickAcid.Bolts;
using QuickAcid.Reporting;

namespace QuickAcid.Tests._Tools.ThePress;

public static class TheJournalist
{
    [StackTraceHidden]
    public static Article Unearths(Report report) => new(report.CaseFile!);

    [StackTraceHidden]
    public static Article Exposes(Action run)
    {
        var ex = Assert.Throws<FalsifiableException>(run);
        return Unearths(ex.QAcidReport);
    }
}
