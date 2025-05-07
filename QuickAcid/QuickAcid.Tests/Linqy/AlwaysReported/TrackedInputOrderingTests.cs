using QuickAcid.Bolts;
using QuickAcid.Bolts.Nuts;
using QuickMGenerate;

namespace QuickAcid.Tests.Linqy.Tracked;

public class TrackedInputOrderingTests
{
    public class Container
    {
        public int Value;
    }

    public class Dependent
    {
        public List<int>? NullList = null;

        public int DoubledValue;
        public Dependent(Container c)
        {
            DoubledValue = c.Value * 2;
            NullList = new List<int>();
        }
    }

    [Fact]
    public void TrackedInput_IndirectReferenceMethodForm_ShouldNotThrow()
    {
        var run =
            from container in "container".Tracked(() => new Container { Value = 21 })
            from dependent in "dependent".Tracked(() => new Dependent(container))
            from act in "we might need an act".Act(() => { })
            from boom in Boom(dependent)
            select Acid.Test;
        var report = new QState(run).ObserveOnce();
        Assert.Null(report);
    }

    private static QAcidRunner<Acid> Boom(Dependent dependent)
    {
        return
            from _ in "spec".Spec(() => dependent.NullList.Count == 0)
            select Acid.Test;
    }

    [Fact]
    public void TrackedInput_IndirectReferenceInline_ShouldSucceed()
    {
        var run =
            from container in "container".Tracked(() => new Container { Value = 21 })
            from dependent in "dependent".Tracked(() => new Dependent(container)) // now container is in scope
            from _ in "spec".Spec(() => dependent.DoubledValue == 42)
            select Acid.Test;

        var report = new QState(run).ObserveOnce();
        Assert.Null(report);
    }
}