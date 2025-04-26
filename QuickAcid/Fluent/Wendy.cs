using QuickAcid.Bolts;
using QuickAcid.CodeGen;
using QuickAcid.Reporting;

namespace QuickAcid.Fluent;

// Let’s wrap this up and get the Report!
public class Wendy
{
    private bool verbose = false;

    private readonly QAcidRunner<Acid> runner;

    public Wendy(QAcidRunner<Acid> runner)
    {
        this.runner = runner;
    }

    public Wendy KeepOneEyeOnTheTouchStone()
    {
        verbose = true;
        return this;
    }

    public QAcidReport AndCheckForGold(int scopes, int executionsPerScope)
    {
        for (int i = 0; i < scopes; i++)
        {
            var state = new QAcidState(runner) { Verbose = verbose };
            state.Run(executionsPerScope);
            if (state.CurrentContext.Failed)
                return state.GetReport();
        }
        return null!;
    }

    public string ToCodeIfFailed(int scopes, int executionsPerScope)
    {
        for (int i = 0; i < scopes; i++)
        {
            var state = new QAcidState(runner) { Verbose = verbose };
            state.Run(executionsPerScope);
            if (state.CurrentContext.Failed)
                return Prospector.Pan(state); ;
        }
        return null!;
    }

    public void AndRunTheWohlwillProcess(int scopes, int executionsPerScope)
    {
        runner.TheWohlwillProcess(scopes, executionsPerScope);
    }
}