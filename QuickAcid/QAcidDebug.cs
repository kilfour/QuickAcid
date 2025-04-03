public static class QAcidDebug
{
    public static Action<string>? Log { get; set; }

    public static void Write(string message)
    {
        Log?.Invoke(message);
    }

    public static void WriteLine(string message)
    {
        Log?.Invoke(message + Environment.NewLine);
    }

    public static void Disable()
    {
        Log = null;
    }

    public static void EnableFileLogging(string fileName = "log.txt")
    {
        var path = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", fileName);
        Log = msg => File.AppendAllText(Path.GetFullPath(path), msg);
    }
    public static QAcidDebugLoggingDisposable Logging() { return new QAcidDebugLoggingDisposable(); }
    public class QAcidDebugLoggingDisposable : IDisposable
    {
        public QAcidDebugLoggingDisposable() { QAcidDebug.EnableFileLogging(); }
        public void Dispose()
        {
            QAcidDebug.Disable();
        }
    }
}

