using QuickAcid.Tests._Tools.ThePress;
using StringExtensionCombinators;

namespace QuickAcid.Tests.Spikes.StoringStuff;

public class StoringALotOfStuffByKeyTests
{
    public enum State { One, Two }

    public class Thing
    {
        public State State { get; private set; } = State.One;

        public void NextState()
        {
            if (State != State.One)
                throw new InvalidOperationException();
            State = State.Two;
        }

        public void DoSomething()
        {
            if (State != State.Two)
                throw new InvalidOperationException();
        }
    }

    [Fact]
    public void First_try()
    {

        var script =
            from things in Script.StashFor<Thing>()
            from choice in Script.ChooseIf(
                (() => things.IsEmpty, "create a thing".Act(() => things.Add(new Thing()))),
                (() => !things.IsEmpty, things.Do("next state", a => true, a => a.NextState()))
                )
            from blowup in "boom".Spec(() => !things.Has(a => a.State == State.Two))
            select Acid.Test;

        var article = TheJournalist.Exposes(() =>
            QState.Run(script)
            .WithOneRun()
            .And(2.ExecutionsPerRun()));

        var actEntry1 = article.Execution(1).Action(1).Read();
        Assert.NotNull(actEntry1);
        Assert.Equal("create a thing", actEntry1.Label);

        var actEntry2 = article.Execution(2).Action(1).Read();
        Assert.NotNull(actEntry2);
        Assert.Equal("next state", actEntry2.Label);

    }
}

