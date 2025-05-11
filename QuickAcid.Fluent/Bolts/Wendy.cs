using QuickAcid.Bolts;
using QuickAcid.Reporting;

namespace QuickAcid.Fluent.Bolts;

// Let’s wrap this up and get the Report!
public class Wendy
{
    private bool verbose = false;

    private readonly QAcidScript<Acid> script;

    public Wendy(QAcidScript<Acid> script)
    {
        this.script = script;
    }

    public Wendy KeepOneEyeOnTheTouchStone()
    {
        verbose = true;
        return this;
    }

    public Report AndCheckForGold(int scopes, int executionsPerScope)
    {
        for (int i = 0; i < scopes; i++)
        {
            var state = new QAcidState(script) { Verbose = verbose };
            state.Observe(executionsPerScope);
            if (state.CurrentContext.Failed)
                return state.GetReport();
        }
        return null!;
    }

    public void ThrowFalsifiableExceptionIfFailed(int scopes, int executionsPerScope)
    {
        for (int i = 0; i < scopes; i++)
        {
            var state = new QAcidState(script) { Verbose = verbose };
            state.Testify(executionsPerScope);
        }
    }
}