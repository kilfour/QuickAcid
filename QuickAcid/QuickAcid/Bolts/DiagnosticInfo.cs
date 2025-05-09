


namespace QuickAcid.Bolts;

public record DiagnosticInfo(string[] Tags, string Message, int PhaseLevel);

public static class Diagnose
{
    public static Action<string, int> This(string[] tags) =>
        (msg, phaseLevel) => DiagnosticContext.Log(new DiagnosticInfo(tags, msg, phaseLevel));
}

public static class DiagnosticContext
{
    [ThreadStatic]
    public static Action<DiagnosticInfo>? Current;

    public static void Log(DiagnosticInfo diagnosticInfo)
    {
        Current?.Invoke(diagnosticInfo);
    }
}