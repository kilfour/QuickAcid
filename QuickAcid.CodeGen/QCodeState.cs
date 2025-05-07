
using QuickAcid;
using QuickAcid.Bolts;
using QuickAcid.CodeGen;

public class QCodeState
{
    private readonly QAcidState state;

    public QCodeState(QAcidRunner<Acid> runner)
    {
        state = new QAcidState(runner);
    }

    public string GenerateCode()
    {
        return GenerateCode(1);
    }

    public string GenerateCode(int executionsPerScope)
    {
        state.Observe(executionsPerScope);
        state.AlwaysReport = true;
        return Prospector.Pan(state);
    }
}
