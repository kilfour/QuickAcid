using System.Diagnostics;
using QuickAcid.Bolts;
using QuickAcid.Proceedings;

namespace QuickAcid.Tests._Tools.ThePress;

public static class TheJournalist
{
    [StackTraceHidden]
    public static Article Unearths(CaseFile caseFile) => new(caseFile!);

    [StackTraceHidden]
    public static Article Exposes(Action run)
    {
        var ex = Assert.Throws<FalsifiableException>(run);
        return Unearths(ex.CaseFile);
    }
}
