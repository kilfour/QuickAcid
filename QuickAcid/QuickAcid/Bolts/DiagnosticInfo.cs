


namespace QuickAcid.Bolts;

public record DiagnosticInfo(string[] Tags, string Message, int PhaseLevel);

public static class DiagnosticContext
{
    [ThreadStatic]
    public static Action<DiagnosticInfo>? Current;

    public static void Log(DiagnosticInfo diagnosticInfo)
    {
        Current?.Invoke(diagnosticInfo);
    }
}