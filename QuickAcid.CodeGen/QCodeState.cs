
using QuickAcid;
using QuickAcid.Bolts;
using QuickAcid.CodeGen;

public class QCodeState
{
    private readonly QAcidState state;

    public QCodeState(QAcidScript<Acid> script)
    {
        state = new QAcidState(script);
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
