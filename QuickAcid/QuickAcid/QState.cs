using QuickAcid.Bolts;
using QuickAcid.Bolts.ShrinkStrats;
using QuickAcid.Bolts.TheyCanFade;
using QuickAcid.CodeGen;
using QuickAcid.MonadiXEtAl;
using QuickAcid.Reporting;

namespace QuickAcid;

public class QState
{
    private readonly QAcidState state;

    public QState(QAcidRunner<Acid> runner)
    {
        state = new QAcidState(runner);
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

    public QAcidReport ObserveOnce()
    {
        return Observe(1);
    }

    public QAcidReport Observe(int executionsPerScope)
    {
        return state.Observe(executionsPerScope);
    }
}
