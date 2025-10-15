using QuickAcid.Tests._Tools.ThePress;
using QuickFuzzr;

namespace QuickAcid.Tests.Bugs;

public class ChooseFromDynamicCollection
{
    public record TheAct : Act;
    public record TheSpec : Spec;
    [Fact]
    public void Example()
    {
        var script =
            from list in Script.Stashed(() => new List<int>() { 0 })
            from input in Script.Execute(Fuzz.Constant(list.Last()))
            from act in Script.Act<TheAct>(() => { list.Add(input + 1); })
            from spec in Script.Spec<TheSpec>(() => !list.Contains(2))
            select Acid.Test;

        var article = TheJournalist.Exposes(() => QState.Run(script)
            .WithOneRun()
            .And(50.ExecutionsPerRun()));

        Assert.Equal(2, article.Execution(1).Read().Times);
        Assert.Equal(1, article.Total().Executions());
        var actionDeposition = article.Execution(1).Action(1).Read();
        Assert.Equal("The act", actionDeposition.Label);
    }

    [Fact]
    public void Trickier()
    {
        var script =
            from list in Script.Stashed(() => new List<int>() { 0 })
            from input in Script.Execute(Fuzz.Constant(list.Last()))
            from act in Script.Act<TheAct>(() => { list.Add(input + 1); })
            from spec in Script.Spec<TheSpec>(() => !list.Contains(2))
            select Acid.Test;

        var article = TheJournalist.Exposes(() => QState.Run(script)
            .WithOneRun()
            .And(50.ExecutionsPerRun()));

        Assert.Equal(2, article.Execution(1).Read().Times);
        Assert.Equal(1, article.Total().Executions());
        var actionDeposition = article.Execution(1).Action(1).Read();
        Assert.Equal("The act", actionDeposition.Label);
    }
}
