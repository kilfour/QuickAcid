using System.Diagnostics;
using QuickAcid.Bolts;
using QuickAcid.Reporting;

namespace QuickAcid;

public static class RunnerExtensions
{
    public static RunCount Runs(this int numberOfRuns) => new(numberOfRuns);
    public static ExecutionCount ExecutionsPerRun(this int numberOfExecutions) => new(numberOfExecutions);
}

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

    private readonly QAcidState state;

    public QState(QAcidScript<Acid> script)
    {
        state = new QAcidState(script);
    }

    public QState(QAcidScript<Acid> script, int seed)
    {
        state = new QAcidState(script, seed);
    }

    public QState ShrinkingActions()
    {
        state.ShrinkingActions = true;
        return this;
    }

    public QState AlwaysReport()
    {
        state.AlwaysReport = true;
        return this;
    }

    public QState Verbose()
    {
        state.Verbose = true;
        return this;
    }

    [StackTraceHidden]
    public void TestifyOnce()
    {
        Testify(1);
    }

    [StackTraceHidden]
    public void Testify(int numberOfExecutions)
    {
        var report = Observe(numberOfExecutions);
        if (report.IsFailed)
            throw new FalsifiableException(report);
    }

    public Report ObserveOnce()
    {
        return Observe(1);
    }

    public Report Observe(int executionsPerScope)
    {
        return state.Observe(executionsPerScope);
    }

    public Report ObserveOnce(Action<QAcidState> action)
    {
        return Observe(1, action);
    }

    public Report Observe(int executionsPerScope, Action<QAcidState> action)
    {
        var report = Observe(executionsPerScope);
        action(state);
        return report;
    }
}
