public static class QAcidDebug
{
    [ThreadStatic]
    private static Action<string>? _log;

    public static void Write(string message) => _log?.Invoke(message);
    public static void WriteLine(string message) => _log?.Invoke(message + Environment.NewLine);

    public static void Disable() => _log = null;

    public static void EnableFileLogging(string fileName = "log.txt")
    {
        var path = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", fileName);
        File.Delete(path);
        _log = msg => File.AppendAllText(Path.GetFullPath(path), msg);
    }

    public static IDisposable Logging(string fileName = "log.txt") =>
        new ScopedLogging(fileName);

    private class ScopedLogging : IDisposable
    {
        public ScopedLogging(string fileName)
        {
            EnableFileLogging(fileName);
        }

        public void Dispose()
        {
            Disable();
        }
    }
}