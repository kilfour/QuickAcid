using QuickPulse.Diagnostics;

namespace QuickAcid.Bolts;

public record DiagnosticInfo(string[] Tags, string Message, int PhaseLevel);

public static class Diagnose
{
    public static Action<string, int> This(string[] tags) =>
        (msg, phaseLevel) => PulseContext.Log(new DiagnosticInfo(tags, msg, phaseLevel));
}
