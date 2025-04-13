namespace QuickAcid.Examples.Tutorial.Chapter3.TheShortestPathtoaBug;

public static class CommandParser
{
    private static readonly Dictionary<string, string> store = new();

    public static void Parse(List<string> tokens)
    {
        if (tokens.Count == 0)
            return;

        var command = tokens[0];

        switch (command)
        {
            case "SET":
                // 🐞 Bug: assumes tokens[1] and tokens[2] exist without checking
                store[tokens[1]] = tokens[2];
                break;

            case "GET":
                var _ = store.TryGetValue(tokens[1], out var _value); // also assumes tokens[1] exists
                break;

            case "DEL":
                store.Remove(tokens[1]); // same problem here
                break;

            default:
                // Unknown command, ignore
                break;
        }
    }

    public static void Reset()
    {
        store.Clear();
    }

    public static string? Get(string key) => store.TryGetValue(key, out var value) ? value : null;
}