
using QuickAcid.Bolts;
using QuickAcid.Bolts.Nuts;
using QuickPulse;
using QuickPulse.Arteries;

namespace QuickAcid.Tests;

public class Spike
{
    [Fact]
    public void QuickPulse_for_running_things()
    {
        var script =
            from a in "act".Act(() => { })
            select Acid.Test;

        var collector = new TheCollector<int>();
        var flow =
            from s in Pulse.Start<QAcidState>()
            from currentExecutionNumber in Pulse.Gather(0)
            from trace in Pulse.Trace(currentExecutionNumber.Value)
            from run in Pulse.Effect(() => script(s))
            from increase in Pulse.Effect(() => currentExecutionNumber.Value++)
            select s;

        var state = new QAcidState(null!);
        var signal = Signal.From(flow).SetArtery(collector);
        //10.Times(() => signal.Pulse(state));
        signal.PulseUntil(() => state.CurrentContext.NeedsToStop() || collector.TheExhibit.Contains(9), state);
        var executionNumbers = collector.TheExhibit;
        Assert.Equal(9, executionNumbers.Last());
    }
}