using QuickPulse;
using QuickPulse.Arteries;


namespace QuickAcid.Bolts;

internal static class Log
{
    private static bool logging = false;

    public static void This(string msg)
    {
        if (logging)
            Signal.Tracing<string>()
               .SetArtery(new WriteDataToFile())
               .Pulse(msg);
    }
}
