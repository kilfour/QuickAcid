public static class QAcidDebug
{
    [ThreadStatic]
    private static Action<string>? _log;

    public static void Write(string message) => _log?.Invoke(message);
    public static void WriteLine(string message) => _log?.Invoke(message + Environment.NewLine);

    public static void Disable() => _log = null;

    public static void EnableFileLogging(string fileName = "log.txt", bool append = false)
    {
        var path = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", fileName);
        if (!append)
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


public interface QLog
{
    void Log(IEnumerable<string> tags, string message, object? data = null);

    void Log(string tag, string message, object? data = null)
        => Log(new[] { tag }, message, data);
}

// public class ConsoleQLog : QLog
// {
//     private readonly HashSet<string> includeTags;

//     public ConsoleQLog(IEnumerable<string>? includeTags = null)
//     {
//         this.includeTags = includeTags != null
//             ? new HashSet<string>(includeTags)
//             : new HashSet<string>();
//     }

//     public void Log(IEnumerable<string> tags, string message, object? data = null)
//     {
//         if (includeTags.Count > 0 && !tags.Any(includeTags.Contains))
//             return;

//         var tagString = string.Join(",", tags);
//         var dataString = data != null ? JsonSerializer.Serialize(data) : "";

//         Console.WriteLine($"[{tagString}] {message}{(data != null ? " " + dataString : "")}");
//     }
// }

public class NullQLog : QLog
{
    public void Log(IEnumerable<string> tags, string message, object? data = null) { }
}

// public class MemoryQLog : QLog
// {
//     public readonly List<string> Entries = new();

//     public void Log(IEnumerable<string> tags, string message, object? data = null)
//     {
//         var tagStr = string.Join(",", tags);
//         var entry = $"[{tagStr}] {message}" + (data != null ? $" {JsonSerializer.Serialize(data)}" : "");
//         Entries.Add(entry);
//     }
// }

public static class QLogExtensions
{
    public static T LogAndReturn<T>(this T value, QLog log, string tag, string message)
    {
        log.Log(tag, message, new { Value = value });
        return value;
    }
}

public sealed class LogScope : IDisposable
{
    private readonly QLog log;
    private readonly string[] tags;
    private readonly string message;

    public LogScope(QLog log, string message, params string[] tags)
    {
        this.log = log;
        this.tags = tags;
        this.message = message;
        log.Log(tags, $"\u2192 Enter: {message}");
    }

    public void Dispose()
    {
        log.Log(tags, $"\u2190 Exit: {message}");
    }
}

public class QAcidLoggingFixture : IDisposable
{
    public QAcidLoggingFixture()
    {
        // Enable file logging globally for debugging
        QAcidDebug.EnableFileLogging();
    }

    public void Dispose()
    {
        QAcidDebug.Disable();
    }
}

// Example usage
// public static class QLogExample
// {
//     public static void RunExample()
//     {
//         QLog logger = new ConsoleQLog(includeTags: new[] { "shrink", "guard:fail" });

//         logger.Log(new[] { "shrink", "input:Quantity" }, "Shrunk candidate", new { From = 3, To = 1 });
//         logger.Log(new[] { "shrink", "guard:fail" }, "Rejected value", new { Attempt = -1, Reason = "Invariant held" });
//         logger.Log(new[] { "trace" }, "This won't be shown unless 'trace' is enabled");

//         42.LogAndReturn(logger, "debug", "Checking value");

//         using (new LogScope(logger, "ShrinkStrategy", "shrink"))
//         {
//             logger.Log("shrink", "Doing work...");
//         }
//     }
// }


// // Stub for QAcidDebug
// public static class QAcidDebug
// {
//     public static void EnableFileLogging() => Console.WriteLine("[debug] File logging enabled.");
//     public static void Disable() => Console.WriteLine("[debug] File logging disabled.");
// }
