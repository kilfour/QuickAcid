using QuickAcid.Bolts;
using QuickAcid.Bolts.Nuts;

namespace QuickAcid.Tests.Linqy.Before;

public class QAcidBeforeSafeTests
{
    public class SimpleSystem
    {
        public bool Flag { get; set; } = false;
    }

    [Fact]
    public void Before_ShouldRunBeforeAction()
    {
        var system = new SimpleSystem();
        var beforeCalledFirst = false;

        var run =
            from _a in "noop".Act(() =>
            {
                beforeCalledFirst = system.Flag;
            }).Before(() =>
            {
                system.Flag = true;
            })
            from _s in "spec".Spec(() => true)
            select Acid.Test;

        var report = new QState(run).ObserveOnce();

        Assert.Null(report);
        Assert.True(beforeCalledFirst); // the flag should be set before Act runs
    }

    [Fact]
    public void Before_ShouldRunOnEachAction()
    {
        var count = 0;

        var run =
            from _a in "noop1".Act(() => { }).Before(() => count++)
            from _b in "noop2".Act(() => { }).Before(() => count++)
            from _s in "spec".Spec(() => true)
            select Acid.Test;

        var report = new QState(run).ObserveOnce();

        Assert.Null(report);
        Assert.Equal(2, count); // both befores should have run
    }
}
