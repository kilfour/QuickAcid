using QuickPulse;
using QuickPulse.Arteries;

namespace QuickAcid.Proceedings.ClerksOffice;

public static class TheClerk
{
    public static string Transcribes(CaseFile caseFile)
    {
        return
            Signal.From(The.CourtStyleGuide)
                .SetArtery(TheString.Catcher())
                .Pulse(caseFile)
                .GetArtery<Holden>()
                .Whispers();
    }
}