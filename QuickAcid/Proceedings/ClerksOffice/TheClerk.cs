using QuickPulse;
using QuickPulse.Arteries;

namespace QuickAcid.Proceedings.ClerksOffice;

public static class TheClerk
{
    public static string Transcribes(CaseFile caseFile)
    {
        return
            Signal.From(The.CourtStyleGuide)
                .SetArtery(Text.Capture())
                .Pulse(caseFile)
                .GetArtery<StringSink>()
                .Content();
    }
}