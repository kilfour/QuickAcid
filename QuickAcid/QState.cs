using System.Diagnostics;
using QuickAcid.RunningThings;

namespace QuickAcid;

public class QState
{
    [StackTraceHidden]
    public static QRunnerConfigurator Run(QAcidScript<Acid> script)
        => Run(null, script, null);

    [StackTraceHidden]
    public static QRunnerConfigurator Run(QAcidScript<Acid> script, int seed)
        => Run(null, script, seed);

    [StackTraceHidden]
    public static QRunnerConfigurator Run(string? testName, QAcidScript<Acid> script, int? seed = null)
        => new(testName, script, seed);
}
