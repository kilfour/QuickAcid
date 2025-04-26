using QuickAcid.CodeGen;
using QuickAcid.Bolts;

namespace QuickAcid.Examples.CodeGen;

public static class GetCodeFrom
{
    public static string This(QAcidRunner<Acid> runner)
    {
        var state = new QAcidState(runner);
        state.Run(1);
        return Prospector.Pan(state);
    }
}
