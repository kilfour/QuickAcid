using QuickAcid.Bolts;
using QuickPulse;

namespace QuickAcid.TestsDeposition._Tools;


public static class Signals
{
    public static Signal<DiagnosticInfo> FilterOnTags(IArtery artery, params string[] filter)
    {
        var flow =
            from _ in Pulse.Using(artery)
            from diagnosis in Pulse.Start<DiagnosticInfo>()
            let needsLogging = diagnosis.Tags.Any(a => filter.Contains(a))
            let indent = new string(' ', Math.Max(0, diagnosis.PhaseLevel) * 4)
            from log in Pulse.TraceIf(needsLogging, $"{indent}{diagnosis.Tags.First()}:{diagnosis.Message}")
            select diagnosis;
        return Signal.From(flow); ;
    }
}