
using QuickAcid.Bolts;
using QuickAcid.Bolts.Nuts;
using QuickPulse;

namespace QuickAcid.Tests;

public class Spike
{
    [Fact]
    public void QuickPulse_for_running_things()
    {
        var script =
            from a in "act".Act(() => { })
            select Acid.Test;

        var flow =
            from state in Pulse.Start<QAcidState>()
            from run in Pulse.Effect(() => script(state))
            select script;
    }
}