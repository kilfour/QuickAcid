using QuickAcid.Bolts;
using QuickAcid.Reporting;

namespace QuickAcid;

public class QState
{
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

    public void TestifyOnce()
    {
        Testify(1);
    }

    public void Testify(int numberOfExecutions)
    {
        state.Testify(numberOfExecutions);
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
